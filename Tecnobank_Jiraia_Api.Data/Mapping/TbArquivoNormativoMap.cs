using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Data.Mapping
{
    public class TbArquivoNormativoMap : IEntityTypeConfiguration<TbArquivoNormativoEntity>
    {
        public void Configure(EntityTypeBuilder<TbArquivoNormativoEntity> builder)
        {
            builder.ToTable("TB_ArquivoNormativo");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Diretorio)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.Status)
                .IsRequired();

            builder.Property(a => a.NomePortaria)
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.EhVisaoEstadual)
                .HasColumnType("bit");

            builder.Property(a => a.TipoPortaria)
                .HasColumnType("int");

            builder.Property(a => a.TipoRegistro)
                .HasColumnType("int");

            builder.Property(a => a.Estado)
                .HasMaxLength(2)
                .HasColumnType("nvarchar(2)");

            builder.Property(a => a.DataVigencia)
                .HasColumnType("datetime");

            builder.Property(a => a.DtHrCriado)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(a => a.DtHrModificado)
                .HasColumnType("datetime");
        }
    }
}
