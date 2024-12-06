using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;

namespace Tecnobank_Jiraia_Api.Domain.Interfaces.Api
{
    public interface IChatGptApi
    {
        Task<string> Chat(List<MensagensDto> listaInteracoes);

        Task<ResumoDocumentoDto> ResumirDocumento(String caminhoArquivo, string extensao);
        Task<ExtracaoInformacaoArquivoNormativoDto> ExtrairInformacaoNormativo(string caminhoArquivo);
    }
}
