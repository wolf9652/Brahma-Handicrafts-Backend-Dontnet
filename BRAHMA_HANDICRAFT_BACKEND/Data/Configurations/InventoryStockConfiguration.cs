using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
    {
        public void Configure(EntityTypeBuilder<InventoryStock> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.LastUpdated).HasColumnType("datetime");

            builder.HasOne(i => i.Product)
                .WithMany(p => p.InventoryStocks)
                .HasForeignKey(i => i.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Size)
                .WithMany(s => s.InventoryStocks)
                .HasForeignKey(i => i.SizeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Design)
                .WithMany(d => d.InventoryStocks)
                .HasForeignKey(i => i.DesignId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
