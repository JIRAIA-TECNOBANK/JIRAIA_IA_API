using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Data.Context;
using Tecnobank_Jiraia_Api.Domain.Entities;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Services;

namespace Tecnobank_Jiraia_Api.Application.Workers
{
    public class ArquivoNormativoWorker : BackgroundService
    {
        private readonly ILogger<ArquivoNormativoWorker> _logger;
        private readonly IChatGptService _chatGptService;
        private readonly RegulatorioContext _regulatorioContext;

        public ArquivoNormativoWorker(
            ILogger<ArquivoNormativoWorker> logger,
            IChatGptService chatGptService,
            RegulatorioContext regulatorioContext)
        {
            _logger = logger;
            _chatGptService = chatGptService;
            _regulatorioContext = regulatorioContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker rodando às: {time}", DateTimeOffset.Now);

                var arquivos = _regulatorioContext.TB_ArquivoNormativo
                    .Where(t => t.Status == StatusArquivoNormativo.Processando)
                    .AsNoTracking();

                var arquivosAtualizados = new List<TbArquivoNormativoEntity>();

                foreach (var arquivo in arquivos)
                {
                    string filePath = arquivo.Diretorio;

                    if (File.Exists(filePath))
                    {
                        var result = await _chatGptService.ExtrairInformacaoNormativo(filePath);

                        if (result != null)
                        {
                            _ = DateTime.TryParse(result.DataVigencia, out DateTime dataVigenciaParsed);

                            arquivosAtualizados.Add(new TbArquivoNormativoEntity
                            {
                                Id = arquivo.Id,
                                Diretorio = arquivo.Diretorio,
                                Status = StatusArquivoNormativo.AguardandoAprovacao,
                                NomePortaria = arquivo.NomePortaria,
                                EhVisaoEstadual = result.EhVisaoEstadual,
                                TipoPortaria = ObterTipoNormativo(result.TipoPortaria),
                                TipoRegistro = ObterTipoRegistro(result.TipoRegistro),
                                DataVigencia = dataVigenciaParsed,
                                Estado = result.Estado.Length > 2 ? null : result.Estado,
                                DtHrCriado = arquivo.DtHrCriado,
                                DtHrModificado = DateTime.Now,
                            });
                        }

                        _logger.LogInformation("Resposta da API: {response}", result);
                    }
                    else
                    {
                        _logger.LogWarning("Arquivo não encontrado: {filePath}", filePath);
                    }
                }

                _regulatorioContext.TB_ArquivoNormativo.UpdateRange(arquivosAtualizados);

                await _regulatorioContext.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private static int? ObterTipoNormativo(string tipoNormativo)
        {
            return tipoNormativo switch
            {
                "Lei" => 1,
                "Resolução" => 2,
                "Decreto" => 3,
                "Portaria" => 4,
                "Edital" => 5,
                "Instrução" => 6,
                "Outros" => 7,
                _ => null,
            };
        }

        private static int? ObterTipoRegistro(string tipoRegistro)
        {
            return tipoRegistro switch
            {
                "Registro de contrato" => 1,
                "Registro de garantia" => 2,
                "Registro de instituição financeira" => 3,
                _ => null,
            };
        }

    }
}
