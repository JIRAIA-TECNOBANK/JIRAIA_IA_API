using Microsoft.EntityFrameworkCore;
using Tecnobank_Jiraia_Api.Data.Mapping;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Data.Context
{
    public class RegulatorioContext : DbContext
    {
        public DbSet<TbChatIaEntity> TB_ChatIA { get; set; }
        public DbSet<TbInteracaoIaEntity> TB_InteracaoIA { get; set; }
        public DbSet<TbArquivoCompiladoEntity> TB_ArquivoCompilado { get; set; }
        public DbSet<TbArquivoNormativoEntity> TB_ArquivoNormativo { get; set; }

        public RegulatorioContext(DbContextOptions<RegulatorioContext> options) : base (options) { }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TbChatIaEntity>(new TbChatIaMap().Configure);
            modelBuilder.Entity<TbInteracaoIaEntity>(new TbInteracaoIaMap().Configure);
            modelBuilder.Entity<TbArquivoCompiladoEntity>(new TbArquivoCompiladoMap().Configure);
            modelBuilder.Entity<TbArquivoNormativoEntity>(new TbArquivoNormativoMap().Configure);
        }
    }
}
