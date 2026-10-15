using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class PromotionBannerConfiguration : IEntityTypeConfiguration<PromotionBanner>
    {
        public void Configure(EntityTypeBuilder<PromotionBanner> builder)
        {
            builder.HasKey(pb => pb.Id);

            builder.Property(pb => pb.ImageURL).HasColumnType("varchar(300)").IsRequired();
            builder.Property(pb => pb.AltText).HasColumnType("varchar(200)").IsRequired();

            builder.HasOne(pb => pb.Promotion)
                .WithMany(p => p.PromotionBanners)
                .HasForeignKey(pb => pb.PromotionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
