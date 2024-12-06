using System.Collections.Generic;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Domain.Repository
{
    public interface IChatGptRepository
    {
        Task<TbChatIaEntity> ConsultarPrimeiraInteracoesPorIdagrupador(int? idAgrupador);

        Task<List<TbChatIaEntity>> ConsultarInteracoesPorIdagrupador(int? idAgrupador);

        Task<TbInteracaoIaEntity> ConsultarResumoDocumento(string consulta);

        Task<int> SalvarInteracaoChat(TbChatIaEntity interacao);

        Task SalvarInteracaoResumoDocumento(TbInteracaoIaEntity interacao);

        Task<List<DadosArquivoDto>> ConsultarArquivosPorListadepalavras(List<string> listaPalavras);
    }
}
