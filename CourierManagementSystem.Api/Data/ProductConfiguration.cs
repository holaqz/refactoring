using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourierManagementSystem.Api.Constants;

namespace CourierManagementSystem.Api.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.Property(p => p.Weight)
                .HasPrecision(AppConstants.weight_precision, AppConstants.weight_scale);

            builder.Property(p => p.Length)
                .HasPrecision(AppConstants.length_precision, AppConstants.length_scale);

            builder.Property(p => p.Width)
                .HasPrecision(AppConstants.width_precision, AppConstants.width_scale);

            builder.Property(p => p.Height)
                .HasPrecision(AppConstants.height_precision, AppConstants.height_scale);
        }
    }
}