using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;

namespace Tecnobank_Jiraia_Api.Domain.Interfaces.Services
{
    public interface IChatGptService
    {
        Task<ResponseDto> Chat(int? idAgrupador, string mensagem);

        Task<ResponseDto> ResumirDocumento(IFormFile documento);

        Task<DadosArquivoResponseDto> EncontrarDocumento(string palavra);
        Task<ExtracaoInformacaoArquivoNormativoDto> ExtrairInformacaoNormativo(string diretorio);
    }
}
