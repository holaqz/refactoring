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
                .HasPrecision(DbConstatnts.weight_precision, DbConstatnts.weight_scale);

            builder.Property(p => p.Length)
                .HasPrecision(DbConstatnts.length_precision, DbConstatnts.length_scale);

            builder.Property(p => p.Width)
                .HasPrecision(DbConstatnts.width_precision, DbConstatnts.width_scale);

            builder.Property(p => p.Height)
                .HasPrecision(DbConstatnts.height_precision, DbConstatnts.height_scale);
        }
    }
}