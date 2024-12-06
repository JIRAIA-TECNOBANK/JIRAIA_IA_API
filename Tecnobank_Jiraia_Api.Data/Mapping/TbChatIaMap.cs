using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tecnobank_Jiraia_Api.Domain.Entities;

namespace Tecnobank_Jiraia_Api.Data.Mapping
{
    public class TbChatIaMap : IEntityTypeConfiguration<TbChatIaEntity>
    {
        public void Configure(EntityTypeBuilder<TbChatIaEntity> builder)
        {
            builder.ToTable("TB_ChatIA");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.IdAgrupador);

            builder.Property(c => c.TipoInteracao)
                   .IsRequired();

            builder.Property(c => c.Interacao)
                   .HasMaxLength(4000)
                   .IsRequired();

            builder.Property(c => c.DtHrInteracao)
                   .IsRequired();
        }
    }
}
