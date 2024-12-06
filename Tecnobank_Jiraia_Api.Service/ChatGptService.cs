using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Entities;
using Tecnobank_Jiraia_Api.Domain.Enum;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Api;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Services;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Util;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Validation;
using Tecnobank_Jiraia_Api.Domain.Repository;

namespace Tecnobank_Jiraia_Api.Service
{
    public class ChatGptService : IChatGptService
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IChatGptValidation _validation;
        private readonly IChatGptRepository _repository;
        private readonly IUtilidade _utilidade;
        private readonly IChatGptApi _chatGptApi;

        public ChatGptService(IHostEnvironment hostingEnvironment,
                              IChatGptValidation validation,
                              IChatGptRepository repository,
                              IUtilidade utilidade, 
                              IChatGptApi chatGptApi)
        {
            _hostingEnvironment = hostingEnvironment;
            _validation = validation;
            _repository = repository;
            _utilidade = utilidade;
            _chatGptApi = chatGptApi;
        }

        public async Task<ResponseDto> Chat(int? idAgrupador, string mensagem)
        {
            List<MensagensDto> listaInteracoes = new();
            string response;

            try
            {
                if (idAgrupador == null)
                {
                    int id = await SalvarInteracaoChat(null, TipoInteracao.Pergunta, mensagem);

                    listaInteracoes = new()
                    {
                        new() { role = "system", content = "Você é um assistente útil." },
                        new() { role = "user", content = mensagem },
                    };

                    response = await _chatGptApi.Chat(listaInteracoes);

                    await SalvarInteracaoChat(id, TipoInteracao.Resposta, response);

                    return new ResponseDto { status = 200, isvalid = true, idAgrupador = id, message = response };
                }
                else
                {
                    await SalvarInteracaoChat(idAgrupador, TipoInteracao.Pergunta, mensagem);

                    TbChatIaEntity primeiraInteracao = await _repository.ConsultarPrimeiraInteracoesPorIdagrupador(idAgrupador);
                    List<TbChatIaEntity> demaisInteracoes = await _repository.ConsultarInteracoesPorIdagrupador(idAgrupador);

                    List<TbChatIaEntity> listaCompleta = new() { primeiraInteracao };
                    listaCompleta.AddRange(demaisInteracoes);

                    listaInteracoes = new()
                    {
                        new() { role = "system", content = "Você é um assistente útil." }
                    };

                    foreach (var item in listaCompleta)
                        listaInteracoes.Add(new() { role = item.TipoInteracao == (int)TipoInteracao.Pergunta ? "user" : "assistant", content = item.Interacao });

                    response = await _chatGptApi.Chat(listaInteracoes);

                    await SalvarInteracaoChat(idAgrupador, TipoInteracao.Resposta, response);

                    return new ResponseDto { status = 200, isvalid = true, idAgrupador = idAgrupador, message = response };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto { status = 400, isvalid = false, message = ex.Message };
            }
        }

        public async Task<ResponseDto> ResumirDocumento(IFormFile documento)
        {
            RetornarDetalhesArquivoDto detalhesArquivo = _validation.ResumirDocumentoValidation(documento);
            string caminhoDocumento = await RealizarUploadDocumento(documento);
            string nomeDocumento = string.Concat(_utilidade.RegexWords("documento", detalhesArquivo.nome), ".", detalhesArquivo.extensao);

            try
            {
                TbInteracaoIaEntity resumoDocumento = await _repository.ConsultarResumoDocumento(nomeDocumento);

                if (resumoDocumento != null)
                    return new ResponseDto { status = 200, isvalid = true, message = resumoDocumento.Resposta };
                else
                {
                    if (string.IsNullOrEmpty(caminhoDocumento))
                        throw new FormatException($"Não foi possível fazer upload do documento informado [{documento.FileName}]");

                    ResumoDocumentoDto response = await _chatGptApi.ResumirDocumento(caminhoDocumento, detalhesArquivo.extensao);

                    await SalvarInteracaoResumoDocumento(nomeDocumento, caminhoDocumento, response.textoOriginal, response.resumo);

                    return !string.IsNullOrEmpty(response.resumo)
                           ? new ResponseDto { status = 200, isvalid = true, message = response.resumo }
                           : new ResponseDto { status = 204, isvalid = true, message = string.Empty };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto { status = 400, isvalid = false, message = ex.Message };
            }
        }

        public async Task<DadosArquivoResponseDto> EncontrarDocumento(string palavra)
        {
            List<string> listaPalavras = _validation.EncontrarDocumento(palavra);
            List<ListaArquivos> listaArquivos = new();

            try
            {
                List<DadosArquivoDto> dadosArquivo = await _repository.ConsultarArquivosPorListadepalavras(listaPalavras);

                if (dadosArquivo != null && dadosArquivo.Count > 0)
                {
                    foreach (var item in dadosArquivo)
                    {
                        ListaArquivos novoItem = new()
                        {
                            nome = item.nome,
                            base64 = _utilidade.RetornarBase64Arquivo(item.diretorio),
                        };
                        listaArquivos.Add(novoItem);
                    }
                }

                return new DadosArquivoResponseDto 
                { 
                    quantidade = dadosArquivo != null && dadosArquivo.Count > 0 ? dadosArquivo.Count : 0, 
                    lista = listaArquivos
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ExtracaoInformacaoArquivoNormativoDto> ExtrairInformacaoNormativo(string diretorio)
        {
            try
            {
                return await _chatGptApi.ExtrairInformacaoNormativo(diretorio);
            }
            catch
            {
                throw;
            }
        }

        private async Task<string> RealizarUploadDocumento(IFormFile documento)
        {
            try
            {
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"Files\", documento.FileName);

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

        private async Task<int> SalvarInteracaoChat(int? idAgrupador, TipoInteracao tipoInteracao, string interacao)
        {
            try
            {
                return await _repository.SalvarInteracaoChat(
                    new TbChatIaEntity()
                    {
                        IdAgrupador = idAgrupador,
                        TipoInteracao = (int)tipoInteracao,
                        Interacao = interacao,
                        DtHrInteracao = DateTime.Now
                    }
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SalvarInteracaoResumoDocumento(string consulta, string caminhoDocumento, string textoOriginal, string resposta)
        {
            try
            {
                await _repository.SalvarInteracaoResumoDocumento(
                    new TbInteracaoIaEntity()
                    {
                        Consulta = consulta,
                        Diretorio = caminhoDocumento,
                        TextoOriginal = textoOriginal,
                        Resposta = resposta,
                        DtHrInteracao = DateTime.Now
                    }
                );
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
