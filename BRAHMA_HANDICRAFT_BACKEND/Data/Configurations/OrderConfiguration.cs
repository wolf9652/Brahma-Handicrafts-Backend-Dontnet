using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.GSTNumber).HasColumnType("varchar(20)");
            builder.Property(o => o.OrderDate).HasColumnType("datetime");
            builder.Property(o => o.TrackingNumber).HasColumnType("varchar(50)").IsRequired();
            builder.Property(o => o.OrderStatus).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(o => o.ItemsSubTotal).HasColumnType("decimal(10,2)");
            builder.Property(o => o.ShippingCharges).HasColumnType("decimal(10,2)");
            builder.Property(o => o.IGST).HasColumnType("decimal(10,2)");
            builder.Property(o => o.CGST).HasColumnType("decimal(10,2)");
            builder.Property(o => o.SGST).HasColumnType("decimal(10,2)");
            builder.Property(o => o.TotalTaxCharges).HasColumnType("decimal(10,2)");
            builder.Property(o => o.GrandTotal).HasColumnType("decimal(10,2)");
            builder.Property(o => o.ShippingAddressJson).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(o => o.BillingAddressJson).HasColumnType("nvarchar(max)").IsRequired();

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
