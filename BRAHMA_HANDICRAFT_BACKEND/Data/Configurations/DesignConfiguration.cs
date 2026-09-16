using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class DesignConfiguration : IEntityTypeConfiguration<Design>
    {
        public void Configure(EntityTypeBuilder<Design> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DesignName).HasColumnType("varchar(150)").IsRequired();
            builder.Property(d => d.DesignDescription).HasColumnType("varchar(500)").IsRequired();

            builder.HasOne(d => d.Product)
                .WithMany(p => p.Designs)
                .HasForeignKey(d => d.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
