using ClosedXML.Excel;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Entities;
using Tecnobank_Jiraia_Api.Domain.Repository;

namespace Tecnobank_Jiraia_Api.Application.Workers
{
    public class ArquivoCompiladoWorker : BackgroundService
    {
        private readonly ILogger<ArquivoCompiladoWorker> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IMonitorNormativoRepository _monitorNormativoRepository;

        public ArquivoCompiladoWorker(
            ILogger<ArquivoCompiladoWorker> logger,
            IHostEnvironment hostEnvironment,
            IMonitorNormativoRepository monitorNormativoRepository)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            _monitorNormativoRepository = monitorNormativoRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker iniciado");

                var compilado = await _monitorNormativoRepository.ObterArquivosCompiladosParaProcessamento(StatusArquivoCompilado.AguardandoProcessamento);

                foreach (var item in compilado)
                {
                    item.Status = StatusArquivoCompilado.Processando;
                    item.DtHrModificado = DateTime.Now;

                    await _monitorNormativoRepository.AtualizarArquivoCompilado(item);

                    var arquivos = ProcessarArquivoExcel(item.Diretorio);

                    foreach (var arquivo in arquivos)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;

                        if (arquivo.Diario != "DOSP")
                            continue;

                        string filePath = null;

                        try
                        {
                            if (!arquivo.Link.Contains("imprensaoficial"))
                            {
                                filePath = await BaixarArquivo(arquivo.Link);

                                await _monitorNormativoRepository.SalvarArquivoNormativo(new TbArquivoNormativoEntity
                                {
                                    NomePortaria = arquivo.Ato,
                                    Diretorio = filePath,
                                    Status = StatusArquivoNormativo.Processando,
                                    DtHrCriado = DateTime.Now
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao baixar arquivo");
                            continue;
                        }
                    }

                    item.Status = StatusArquivoCompilado.ProcessadoComSucesso;
                    item.DtHrModificado = DateTime.Now;

                    await _monitorNormativoRepository.AtualizarArquivoCompilado(item);
                }

                _logger.LogInformation("Worker finalizado");

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private static List<ArquivoNormativoProcessadoDto> ProcessarArquivoExcel(string excelPath)
        {
            var arquivos = new List<ArquivoNormativoProcessadoDto>();

            using (var workbook = new XLWorkbook(excelPath))
            {
                var worksheet = workbook.Worksheet(1);

                foreach (var row in worksheet.RowsUsed().Skip(2))
                {
                    arquivos.Add(new ArquivoNormativoProcessadoDto
                    {
                        Data = row.Cell(1).GetString(),
                        Diario = row.Cell(2).GetString(),
                        Ato = row.Cell(3).GetString(),
                        Link = row.Cell(4).GetString(),
                        Observacoes = row.Cell(7).GetString(),
                        DataVigencia = row.Cell(8).GetString(),
                        Impacto = row.Cell(9).GetString()
                    });
                }
            }

            return arquivos;
        }

        public async Task<string> BaixarArquivo(string url)
        {
            using var playwright = await Playwright.CreateAsync();

            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();

            await page.GotoAsync(url);

            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, @"Files\Normativo", Guid.NewGuid() + ".pdf");

            //await page.WaitForSelectorAsync("#__next > div.css-1kz6g9a > div > div.css-4gzfmr > div > a");
            await page.WaitForLoadStateAsync(LoadState.Load);

            await page.Locator("#__next > div.css-1kz6g9a > div > div.css-4gzfmr > div > a").ClickAsync();

            await Task.Delay(TimeSpan.FromSeconds(5));

            var newPage = context.Pages.Where(p => p.Url.Contains("blob:")).FirstOrDefault();

            var blobContent = await newPage.EvaluateAsync<string>(@"
                    () => {
                        return new Promise((resolve, reject) => {
                            fetch(document.location.href)
                            .then(response => response.blob())
                            .then(blob => {
                                var reader = new FileReader();
                                reader.onload = function() {
                                    resolve(reader.result);
                                };
                                reader.onerror = function() {
                                    reject('Falha ao ler o blob.');
                                };
                                reader.readAsDataURL(blob);
                            })
                            .catch(err => reject('Erro ao buscar o blob: ' + err));
                        });
                    }
                ");

            string base64Data = blobContent.Split(',')[1];
            byte[] fileBytes = Convert.FromBase64String(base64Data);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            await browser.CloseAsync();

            return filePath;
        }
    }
}
