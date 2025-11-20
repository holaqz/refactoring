using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierManagementSystem.Api.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasIndex(u => u.Login)
                .IsUnique();

            builder.Property(u => u.Role)
                .HasConversion<string>();

            // сид админа
            builder.HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    // BCrypt hash of "admin123" - same as in Liquibase migration
                    PasswordHash = "$2a$10$z1azzGeYiaHewbX.R5XQb.9WzRldo.ER6S749OswSTtGh.E.FORSG",
                    Name = "Системный администратор",
                    Role = UserRole.admin,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}