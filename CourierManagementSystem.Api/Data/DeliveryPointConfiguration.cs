using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                .HasPrecision(DbConstatnts.latitude_precision, DbConstatnts.Latitude_scale);

            builder.Property(dp => dp.Longitude)
                .HasPrecision(DbConstatnts.longitude_precision, DbConstatnts.longitude_scale);

            // Configure relationship with DeliveryPointProducts
            builder.HasMany(dp => dp.DeliveryPointProducts)
                .WithOne(dpp => dpp.DeliveryPoint)
                .HasForeignKey(dpp => dpp.DeliveryPointId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}