using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierManagementSystem.Api.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.Property(p => p.Weight)
                .HasPrecision(8, 2);

            builder.Property(p => p.Length)
                .HasPrecision(6, 2);

            builder.Property(p => p.Width)
                .HasPrecision(6, 2);

            builder.Property(p => p.Height)
                .HasPrecision(6, 2);
        }
    }
}