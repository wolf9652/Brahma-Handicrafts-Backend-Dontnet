using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasColumnType("varchar(150)").IsRequired();
            builder.Property(p => p.Description).HasColumnType("varchar(500)").IsRequired();
            builder.Property(p => p.DefaultPrice).HasColumnType("decimal(10,2)");
            builder.Property(p => p.DefaultMRP).HasColumnType("decimal(10,2)");
            builder.Property(p => p.IsActive).HasDefaultValue(true);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
