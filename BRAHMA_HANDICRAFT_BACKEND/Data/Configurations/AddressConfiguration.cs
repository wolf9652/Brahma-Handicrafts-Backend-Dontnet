using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AddressLine1).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(a => a.City).HasColumnType("varchar(100)").IsRequired();
            builder.Property(a => a.State).HasColumnType("varchar(100)").IsRequired();
            builder.Property(a => a.PostalCode).HasColumnType("varchar(20)").IsRequired();
            builder.Property(a => a.Country).HasColumnType("varchar(100)").IsRequired();
            builder.Property(a => a.IsActive).HasDefaultValue(true);

            builder.HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
