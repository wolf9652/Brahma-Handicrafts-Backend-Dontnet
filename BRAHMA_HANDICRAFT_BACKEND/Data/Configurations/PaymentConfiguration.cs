using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PaymentMethod).HasColumnType("varchar(50)").IsRequired();
            builder.Property(p => p.Status).HasColumnType("varchar(50)").IsRequired();
            builder.Property(p => p.Date).HasColumnType("datetime");
            builder.Property(p => p.Amount).HasColumnType("decimal(10,2)");
            builder.Property(p => p.TransactionId).HasColumnType("varchar(100)").IsRequired();
            builder.Property(p => p.ResponseCode).HasColumnType("varchar(20)");
            builder.Property(p => p.ErrorMessage).HasColumnType("varchar(255)");

            builder.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
