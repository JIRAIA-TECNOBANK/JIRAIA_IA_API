using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Data.Mapping
{
    public class TbArquivoCompiladoMap : IEntityTypeConfiguration<TbArquivoCompiladoEntity>
    {
        public void Configure(EntityTypeBuilder<TbArquivoCompiladoEntity> builder)
        {
            builder.ToTable("TB_ArquivoCompilado");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nome)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.Diretorio)
                .IsRequired()
                .HasColumnType("NVARCHAR(MAX)");

            builder.Property(a => a.Status)
                .IsRequired();

            builder.Property(a => a.DtHrCriado)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.DtHrModificado)
                .IsRequired(false);
        }
    }
}
