using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Domain.Interfaces.Services
{
    public interface IMonitorNormativoService
    {
        Task EnviarArquivoCompilado(IFormFile arquivo);
        Task<List<ArquivoCompiladoResponseDto>> ObterListaArquivoCompilado();
        Task<List<ArquivoNormativoResponseDto>> ObterListaArquivoNormativoAprovacao();
        Task<List<ArquivoNormativoResponseDto>> ObterListaArquivoNormativoAprovados();
        Task<List<ArquivoNormativoResponseDto>> ObterListaArquivoNormativoRejeitado();
        Task<TbArquivoNormativoEntity> AprovarArquivoNormativo(int id);
        Task<TbArquivoNormativoEntity> RejeitarArquivoNormativo(int id);
        Task<DadosArquivoResponseDto> ObterArquivoNormativo(int id);
    }
}
