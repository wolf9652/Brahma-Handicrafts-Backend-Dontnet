using BRAHMA_HANDICRAFT_BACKEND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRAHMA_HANDICRAFT_BACKEND.Data.Configurations
{
    public class UsersConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName).HasColumnType("varchar(100)").IsRequired();
            builder.Property(u => u.LastName).HasColumnType("varchar(100)").IsRequired();
            builder.Property(u => u.PasswordHash).HasColumnType("varchar(255)").IsRequired();
            builder.Property(u => u.PhoneNumber).HasColumnType("varchar(20)").IsRequired();
            builder.Property(u => u.EmailId).HasColumnType("varchar(255)").IsRequired();
            builder.Property(u => u.Role).HasDefaultValue(true);
        }
    }
}
