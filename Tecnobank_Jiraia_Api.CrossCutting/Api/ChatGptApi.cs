using iText.Kernel.Pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Api;

namespace Tecnobank_Jiraia_Api.CrossCutting.Api
{
    public class ChatGptApi : IChatGptApi
    {
        private readonly IConfiguration _configuration;
        private readonly Configuration _configuracoes;
        private readonly ILogger<ChatGptApi> _logger;
        private string message = string.Empty;

        public ChatGptApi(IConfiguration configuration, ILogger<ChatGptApi> logger)
        {
            _configuration = configuration;
            _configuracoes = new Configuration(_configuration);
            _logger = logger;
        }

        public async Task<string> Chat(List<MensagensDto> listaInteracoes)
        {
            RestResponse response = new();

            try
            {
                for (int attempt = 0; attempt < _configuracoes.ChatGptMaxRetries; attempt++)
                {
                    RestClient client = new(_configuracoes.ChatGptApiUrlCompletions);
                    RestRequest request = new("", Method.Post);
                    request.AddHeader("Authorization", $"Bearer {_configuracoes.ChatGptApiKey}");
                    request.AddHeader("Content-Type", "application/json");

                    var body = new
                    {
                        model = _configuracoes.ChatGptModel,
                        messages = listaInteracoes,
                        max_tokens = _configuracoes.ChatGptMaxTokens,
                        temperature = _configuracoes.ChatGptTemperature
                    };

                    request.AddJsonBody(body);

                    response = await client.ExecuteAsync(request);

                    if (response.IsSuccessful)
                    {
                        ChatResponseDto myDeserializedClass = JsonConvert.DeserializeObject<ChatResponseDto>(response.Content);
                        message += myDeserializedClass.choices[0].message.content;

                        return message;
                    }
                    else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        message += $"Muitas requisições. Tentando novamente...";
                        await Task.Delay(_configuracoes.ChatGptMaxDelay);

                        return message;
                    }
                    else
                    {
                        message += $"Erro: {response.StatusCode} - {response.Content}";

                        return message;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return message;
        }

        public async Task<ResumoDocumentoDto> ResumirDocumento(String caminhoArquivo, string extensao)
        {
            try
            {
                using (HttpClient client = new())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracoes.ChatGptApiKey}");

                    var requestData = new
                    {
                        model = _configuracoes.ChatGptModel,
                        messages = new[] {
                            new {
                                role = "system",
                                content = "Você é um assistente útil."
                            },
                            new {
                                role = "user",
                                content = RetornarConteudo(caminhoArquivo, extensao)
                            }
                        },
                        max_tokens = _configuracoes.ChatGptMaxTokens,
                        temperature = _configuracoes.ChatGptTemperature
                    };

                    string jsonRequest = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_configuracoes.ChatGptApiUrlCompletions, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        string textoOriginal = RetornarConteudo(caminhoArquivo, extensao).Replace("Resuma o texto destacando os principais pontos: ", "");
                        ChatResponseDto myDeserializedClass = JsonConvert.DeserializeObject<ChatResponseDto>(responseBody);
                        message += myDeserializedClass.choices[0].message.content;

                        return new ResumoDocumentoDto { textoOriginal = textoOriginal, resumo = message };
                    }
                    else
                        throw new Exception($"Erro: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ExtracaoInformacaoArquivoNormativoDto> ExtrairInformacaoNormativo(string caminhoArquivo)
        {
            try
            {
                using HttpClient client = new();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracoes.ChatGptApiKey}");

                var requestData = new
                {
                    model = _configuracoes.ChatGptModel,
                    messages = new[]
                    {
                            new { role = "system", content = "Você é um assistente que extrai informações em um formato JSON." },
                            new { role = "user", content = RetornarConteudoNormativo(caminhoArquivo) }
                    },
                    max_tokens = _configuracoes.ChatGptMaxTokens,
                    temperature = _configuracoes.ChatGptTemperature
                };

                string jsonRequest = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_configuracoes.ChatGptApiUrlCompletions, content);

                var responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    ChatResponseDto myDeserializedClass = JsonConvert.DeserializeObject<ChatResponseDto>(responseBody);

                    var arquivoNormativo = JsonConvert.DeserializeObject<ExtracaoInformacaoArquivoNormativoDto>(myDeserializedClass.choices[0].message.content);

                    if (arquivoNormativo != null)
                    {
                        return arquivoNormativo;
                    }
                    else
                    {
                        _logger.LogError("Erro: ArquivoNormativoDto retornado nulo.");
                        return null;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError("Erro ao processar o JSON retornado: {error}", ex.Message);
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string RetornarConteudo(String caminhoArquivo, string extensao)
        {
            if (extensao.Equals("txt"))
                return $"Resuma o texto destacando os principais pontos: {File.ReadAllText(caminhoArquivo)}";
            else
            {
                StringBuilder texto = new();

                using (PdfReader pdfReader = new(caminhoArquivo))
                using (PdfDocument pdfDocument = new(pdfReader))
                {
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        var page = pdfDocument.GetPage(i);
                        var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy();
                        var text = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page, strategy);
                        texto.AppendLine(text);
                    }

                    return $"Resuma o texto destacando os principais pontos: {texto}";
                }
            }
        }

        private static string RetornarConteudoNormativo(string caminhoArquivo)
        {
            StringBuilder texto = new();

            using PdfReader pdfReader = new(caminhoArquivo);
            using PdfDocument pdfDocument = new(pdfReader);

            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy();
                var text = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page, strategy);
                texto.AppendLine(text);
            }

            return $"Resuma o texto e extraia as seguintes informações em JSON (valores podem ser nulos se não encontrados): EhVisaoEstadual, TipoPortaria (Identificar se é Lei, Resolução, Decreto, Portaria, Edital, Instrução ou Outros), TipoRegistro (Registro de contrato, Registro de garantia ou Registro de instituição financeira), Estado(Deve retornar somente a sigla como SP e RJ) e DataVigencia (DataVigencia deve ser um formato de data como yyyy-MM-ddTHH:mm:ss). O texto é: {texto}";
        }
    }
}
