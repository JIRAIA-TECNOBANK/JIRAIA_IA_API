using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Data.Context;
using Tecnobank_Jiraia_Api.Domain.Dto;
using Tecnobank_Jiraia_Api.Domain.Entities;
using Tecnobank_Jiraia_Api.Domain.Enum;
using Tecnobank_Jiraia_Api.Domain.Repository;

namespace Tecnobank_Jiraia_Api.Data.Repository
{
    public class ChatGptRepository : IChatGptRepository
    {
        private readonly RegulatorioContext _regulatorioContext;

        public ChatGptRepository(RegulatorioContext regulatorioContext)
        {
            _regulatorioContext = regulatorioContext;
        }

        public async Task<TbChatIaEntity> ConsultarPrimeiraInteracoesPorIdagrupador(int? idAgrupador)
        {
            try
            {
                return await _regulatorioContext.TB_ChatIA.Where(c => c.Id == idAgrupador).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TbChatIaEntity>> ConsultarInteracoesPorIdagrupador(int? idAgrupador)
        {
            try
            {
                return await _regulatorioContext.TB_ChatIA.Where(c => c.Id != idAgrupador && c.IdAgrupador == idAgrupador).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TbInteracaoIaEntity> ConsultarResumoDocumento(string consulta)
        {
            try
            {
                return await _regulatorioContext.TB_InteracaoIA.Where(iia => iia.Consulta.Equals(consulta.ToLower())).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> SalvarInteracaoChat(TbChatIaEntity interacao)
        {
            try
            {
                _regulatorioContext.TB_ChatIA.Add(interacao);
                await _regulatorioContext.SaveChangesAsync();

                return interacao.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SalvarInteracaoResumoDocumento(TbInteracaoIaEntity interacao)
        {
            try
            {
                _regulatorioContext.TB_InteracaoIA.Add(interacao);
                await _regulatorioContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<DadosArquivoDto>> ConsultarArquivosPorListadepalavras(List<string> listaPalavras)
        {
            StringBuilder sql = new();
            sql.Append($"SELECT IIA.* ");
            sql.Append($"  FROM [Regulatorio1].[dbo].[TB_InteracaoIA] IIA ");
            sql.Append($" WHERE ( ");

            for (int i = 0; i < listaPalavras.Count; i++)
            {
                sql.Append($"LOWER(IIA.TextoOriginal) LIKE '%{listaPalavras[i].ToLower()}%' ");

                if (i < listaPalavras.Count - 1)
                    sql.Append($" OR ");
            }

            sql.Append($") ");

            try
            {
                List<TbInteracaoIaEntity> listaInteracao = await _regulatorioContext.TB_InteracaoIA.FromSqlRaw(sql.ToString()).AsNoTracking().ToListAsync();

                if (listaInteracao != null && listaInteracao.Count > 0)
                {
                    List<DadosArquivoDto> listaArquivos = new();

                    foreach (var item in listaInteracao)
                    {
                        DadosArquivoDto novoItem = new()
                        {
                            nome = item.Consulta,
                            diretorio = item.Diretorio
                        };
                        listaArquivos.Add(novoItem);
                    }

                    return listaArquivos;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
