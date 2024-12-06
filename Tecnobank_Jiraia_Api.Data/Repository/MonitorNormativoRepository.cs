using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tecnobank_Jiraia_Api.Data.Context;
using Tecnobank_Jiraia_Api.Domain.Entities;
using Tecnobank_Jiraia_Api.Domain.Repository;

namespace Tecnobank_Jiraia_Api.Data.Repository
{
    public class MonitorNormativoRepository : IMonitorNormativoRepository
    {
        private readonly RegulatorioContext _regulatorioContext;

        public MonitorNormativoRepository(RegulatorioContext regulatorioContext)
        {
            _regulatorioContext = regulatorioContext;
        }

        public async Task<List<TbArquivoCompiladoEntity>> ObterArquivosCompilados()
        {
            try
            {
                return await _regulatorioContext.TB_ArquivoCompilado.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SalvarArquivoCompilado(TbArquivoCompiladoEntity arquivoCompilado)
        {
            try
            {
                _regulatorioContext.TB_ArquivoCompilado.Add(arquivoCompilado);
                await _regulatorioContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task AtualizarArquivoCompilado(TbArquivoCompiladoEntity arquivoCompilado)
        {
            try
            {
                _regulatorioContext.TB_ArquivoCompilado.Update(arquivoCompilado);
                await _regulatorioContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SalvarArquivoNormativo(TbArquivoNormativoEntity arquivoNormativo)
        {
            try
            {
                _regulatorioContext.TB_ArquivoNormativo.Add(arquivoNormativo);
                await _regulatorioContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TbArquivoNormativoEntity>> ObterListaArquivoCompilado(StatusArquivoNormativo status)
        {
            try
            {
                return await _regulatorioContext.TB_ArquivoNormativo.Where(s => s.Status == status).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TbArquivoCompiladoEntity>> ObterArquivosCompiladosParaProcessamento(StatusArquivoCompilado status)
        {
            try
            {
                return await _regulatorioContext.TB_ArquivoCompilado.Where(s => s.Status == status).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TbArquivoNormativoEntity> AtualizarArquivoNormativo(int id, StatusArquivoNormativo status)
        {
            try
            {
                var normativo = await _regulatorioContext.TB_ArquivoNormativo.Where(t => t.Id == id).FirstOrDefaultAsync();

                normativo.Status = status;

                _regulatorioContext.TB_ArquivoNormativo.Update(normativo);

                await _regulatorioContext.SaveChangesAsync();

                return normativo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TbArquivoNormativoEntity> ObterArquivoNormativo(int id)
        {
            try
            {
                var normativo = await _regulatorioContext.TB_ArquivoNormativo.Where(t => t.Id == id).FirstOrDefaultAsync();

                return normativo;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
