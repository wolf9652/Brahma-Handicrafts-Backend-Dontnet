using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasColumnType("varchar(150)").IsRequired();
            builder.Property(p => p.DiscountType).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(p => p.DiscountValue).HasColumnType("decimal(10,2)");
        }
    }
}
