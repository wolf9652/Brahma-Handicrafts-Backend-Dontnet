using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class SizeConfiguration : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SizeName).HasColumnType("varchar(50)").IsRequired();
            builder.Property(s => s.Length).HasColumnType("decimal(10,2)");
            builder.Property(s => s.Breadth).HasColumnType("decimal(10,2)");
            builder.Property(s => s.Height).HasColumnType("decimal(10,2)");
            builder.Property(s => s.Weight).HasColumnType("decimal(10,2)");
            builder.Property(s => s.BasePrice).HasColumnType("decimal(10,2)");
            builder.Property(s => s.MRP).HasColumnType("decimal(10,2)");

            builder.HasOne(s => s.Product)
                .WithMany(p => p.Sizes)
                .HasForeignKey(s => s.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Design)
                .WithMany(d => d.Sizes)
                .HasForeignKey(s => s.DesignId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
