using System.Collections.Generic;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Domain.Repository
{
    public interface IMonitorNormativoRepository
    {
        Task SalvarArquivoCompilado(TbArquivoCompiladoEntity arquivoCompilado);
        Task AtualizarArquivoCompilado(TbArquivoCompiladoEntity arquivoCompilado);
        Task SalvarArquivoNormativo(TbArquivoNormativoEntity arquivoNormativo);
        Task<List<TbArquivoCompiladoEntity>> ObterArquivosCompilados();
        Task<List<TbArquivoCompiladoEntity>> ObterArquivosCompiladosParaProcessamento(StatusArquivoCompilado status);
        Task<List<TbArquivoNormativoEntity>> ObterListaArquivoCompilado(StatusArquivoNormativo status);
        Task<TbArquivoNormativoEntity> AtualizarArquivoNormativo(int id, StatusArquivoNormativo status);
        Task<TbArquivoNormativoEntity> ObterArquivoNormativo(int id);
    }
}
