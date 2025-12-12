using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourierManagementSystem.Api.Constants;

namespace CourierManagementSystem.Api.Data.Configurations
{
    public class DeliveryPointConfiguration : IEntityTypeConfiguration<DeliveryPoint>
    {
        public void Configure(EntityTypeBuilder<DeliveryPoint> builder)
        {
            builder.ToTable("delivery_points");

            // Unique constraint on DeliveryId and Sequence
            builder.HasIndex(dp => new { dp.DeliveryId, dp.Sequence })
                .IsUnique();

            builder.Property(dp => dp.Latitude)
                .HasPrecision(AppConstants.latitude_precision, AppConstants.Latitude_scale);

            builder.Property(dp => dp.Longitude)
                .HasPrecision(AppConstants.longitude_precision, AppConstants.longitude_scale);

            // Configure relationship with DeliveryPointProducts
            builder.HasMany(dp => dp.DeliveryPointProducts)
                .WithOne(dpp => dpp.DeliveryPoint)
                .HasForeignKey(dpp => dpp.DeliveryPointId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}