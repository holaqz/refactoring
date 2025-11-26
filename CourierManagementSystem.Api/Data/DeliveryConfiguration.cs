using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierManagementSystem.Api.Data.Configurations
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.ToTable("deliveries");

            builder.Property(d => d.Status)
                .HasConversion<string>();

            // Configure relationship with Courier (User)
            builder.HasOne(d => d.Courier)
                .WithMany()
                .HasForeignKey(d => d.CourierId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationship with Vehicle
            builder.HasOne(d => d.Vehicle)
                .WithMany()
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationship with CreatedBy (User)
            builder.HasOne(d => d.CreatedBy)
                .WithMany()
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship with DeliveryPoints
            builder.HasMany(d => d.DeliveryPoints)
                .WithOne(dp => dp.Delivery)
                .HasForeignKey(dp => dp.DeliveryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}