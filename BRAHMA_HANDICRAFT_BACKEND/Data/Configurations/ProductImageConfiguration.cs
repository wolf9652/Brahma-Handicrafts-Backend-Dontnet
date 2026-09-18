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

            // DesignId is nullable: a Product without Designs must still be able to have images (Case 1/2).
            builder.HasOne(pi => pi.Design)
                .WithMany(d => d.ProductImages)
                .HasForeignKey(pi => pi.DesignId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
