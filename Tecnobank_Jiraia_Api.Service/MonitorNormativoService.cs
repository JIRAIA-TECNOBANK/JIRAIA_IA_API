using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Entities;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Services;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Util;
using Tecnobank_Jiraia_Api.Domain.Repository;

namespace Tecnobank_Jiraia_Api.Service
{
    public class MonitorNormativoService : IMonitorNormativoService
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IUtilidade _utilidade;
        private readonly IMonitorNormativoRepository _monitorNormativoRepository;

        public MonitorNormativoService(
            IHostEnvironment hostingEnvironment,
            IUtilidade utilidade, 
            IMonitorNormativoRepository monitorNormativoRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _utilidade = utilidade;
            _monitorNormativoRepository = monitorNormativoRepository;
        }

        public async Task EnviarArquivoCompilado(IFormFile arquivo)
        {
            try
            {
                string diretorio = await RealizarUploadArquivoCompilado(arquivo);

                await SalvarArquivoCompilado(arquivo.FileName, diretorio);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<ArquivoCompiladoResponseDto>> ObterListaArquivoCompilado()
        {
            var response = new List<ArquivoCompiladoResponseDto>();

            try
            {
                var entitities = await _monitorNormativoRepository.ObterArquivosCompilados();

                foreach (var item in entitities)
                {
                    response.Add(new ArquivoCompiladoResponseDto
                    {
                        Nome = item.Nome,
                        Status = item.Status,
                        DtHrCriado = item.DtHrCriado,
                        DtHrModificado = item.DtHrModificado,
                    });
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ArquivoNormativoResponseDto>> ObterListaArquivoNormativoAprovacao()
        {
            var response = new List<ArquivoNormativoResponseDto>();

            try
            {
                var entitities = await _monitorNormativoRepository.ObterListaArquivoCompilado(StatusArquivoNormativo.AguardandoAprovacao);

                foreach (var item in entitities)
                {
                    response.Add(new ArquivoNormativoResponseDto
                    {
                        Id = item.Id,
                        NomePortaria = item.NomePortaria,
                        Status = item.Status,
                        Estado = item.Estado,
                        TipoPortaria = item.TipoPortaria.HasValue ? item.TipoPortaria.Value : null,
                        TipoRegistro = item.TipoRegistro.HasValue ? item.TipoRegistro.Value : null,
                        DataVigencia = item.DataVigencia.HasValue ? item.DataVigencia.Value : null,
                        EhVisaoEstadual = item.EhVisaoEstadual.HasValue ? item.EhVisaoEstadual.Value : null,
                        DtHrCriado = item.DtHrCriado,
                        DtHrModificado = item.DtHrModificado
                    });
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ArquivoNormativoResponseDto>> ObterListaArquivoNormativoAprovados()
        {
            var response = new List<ArquivoNormativoResponseDto>();

            try
            {
                var entitities = await _monitorNormativoRepository.ObterListaArquivoCompilado(StatusArquivoNormativo.Aprovado);

                foreach (var item in entitities)
                {
                    response.Add(new ArquivoNormativoResponseDto
                    {
                        Id = item.Id,
                        NomePortaria = item.NomePortaria,
                        Status = item.Status,
                        Estado = item.Estado,
                        TipoPortaria = item.TipoPortaria.HasValue ? item.TipoPortaria.Value : null,
                        TipoRegistro = item.TipoRegistro.HasValue ? item.TipoRegistro.Value : null,
                        DataVigencia = item.DataVigencia.HasValue ? item.DataVigencia.Value : null,
                        EhVisaoEstadual = item.EhVisaoEstadual.HasValue ? item.EhVisaoEstadual.Value : null,
                        DtHrCriado = item.DtHrCriado,
                        DtHrModificado = item.DtHrModificado
                    });
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ArquivoNormativoResponseDto>> ObterListaArquivoNormativoRejeitado()
        {
            var response = new List<ArquivoNormativoResponseDto>();

            try
            {
                var entitities = await _monitorNormativoRepository.ObterListaArquivoCompilado(StatusArquivoNormativo.Rejeitado);

                foreach (var item in entitities)
                {
                    response.Add(new ArquivoNormativoResponseDto
                    {
                        Id = item.Id,
                        NomePortaria = item.NomePortaria,
                        Status = item.Status,
                        Estado = item.Estado,
                        TipoPortaria = item.TipoPortaria.HasValue ? item.TipoPortaria.Value : null,
                        TipoRegistro = item.TipoRegistro.HasValue ? item.TipoRegistro.Value : null,
                        DataVigencia = item.DataVigencia.HasValue ? item.DataVigencia.Value : null,
                        EhVisaoEstadual = item.EhVisaoEstadual.HasValue ? item.EhVisaoEstadual.Value : null,
                        DtHrCriado = item.DtHrCriado,
                        DtHrModificado = item.DtHrModificado
                    });
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SalvarArquivoCompilado(string nomeArquivo, string diretorio)
        {
            try
            {
                await _monitorNormativoRepository.SalvarArquivoCompilado(
                    new TbArquivoCompiladoEntity()
                    {
                        Nome = nomeArquivo,
                        Diretorio = diretorio,
                        Status = StatusArquivoCompilado.AguardandoProcessamento,
                    }
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> RealizarUploadArquivoCompilado(IFormFile documento)
        {
            try
            {
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"Files\Compilado", documento.FileName);

                using (FileStream filestream = File.Create(path))
                {
                    await documento.CopyToAsync(filestream);
                    filestream.Flush();
                }

                return path;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<TbArquivoNormativoEntity> AprovarArquivoNormativo(int id)
        {
            try
            {
                var response = await _monitorNormativoRepository.AtualizarArquivoNormativo(id, StatusArquivoNormativo.Aprovado);

                return response;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TbArquivoNormativoEntity> RejeitarArquivoNormativo(int id)
        {
            try
            {
                var response = await _monitorNormativoRepository.AtualizarArquivoNormativo(id, StatusArquivoNormativo.Rejeitado);

                return response;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DadosArquivoResponseDto> ObterArquivoNormativo(int id)
        {
            try
            {
                var normativo = await _monitorNormativoRepository.ObterArquivoNormativo(id);

                ListaArquivos arquivo = null;

                if (normativo != null)
                {
                    arquivo = new ListaArquivos
                    {
                        nome = normativo.NomePortaria + ".pdf",
                        base64 = _utilidade.RetornarBase64Arquivo(normativo.Diretorio),
                    };
                }

                return new DadosArquivoResponseDto
                {
                    quantidade = arquivo != null ? 1 : 0,
                    lista = arquivo != null ? new List<ListaArquivos> { arquivo } : new List<ListaArquivos>()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
