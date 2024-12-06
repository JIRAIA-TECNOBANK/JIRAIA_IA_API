using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Data.Mapping
{
    public class TbInteracaoIaMap : IEntityTypeConfiguration<TbInteracaoIaEntity>
    {
        public void Configure(EntityTypeBuilder<TbInteracaoIaEntity> builder)
        {
            builder.ToTable("TB_InteracaoIA");

            builder.HasKey(iia => iia.Id);

            builder.Property(iia => iia.Consulta)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(iia => iia.Diretorio);

            builder.Property(iia => iia.TextoOriginal);

            builder.Property(iia => iia.Resposta)
                   .IsRequired();

            builder.Property(iia => iia.DtHrInteracao)
                   .IsRequired();
        }
    }
}
