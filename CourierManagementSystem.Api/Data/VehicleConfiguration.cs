using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierManagementSystem.Api.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("vehicles");

            builder.HasIndex(v => v.LicensePlate)
                .IsUnique();

            builder.Property(v => v.MaxWeight)
                .HasPrecision(8, 2);

            builder.Property(v => v.MaxVolume)
                .HasPrecision(8, 3);
        }
    }
}