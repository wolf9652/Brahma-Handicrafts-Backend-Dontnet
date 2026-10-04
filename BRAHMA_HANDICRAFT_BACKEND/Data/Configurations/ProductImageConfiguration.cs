using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.ImageURL).HasColumnType("varchar(500)").IsRequired();
            builder.Property(pi => pi.AltText).HasColumnType("varchar(150)").IsRequired();

            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Every Product always has at least one Design, so every image belongs to a Design.
            builder.HasOne(pi => pi.Design)
                .WithMany(d => d.ProductImages)
                .HasForeignKey(pi => pi.DesignId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
