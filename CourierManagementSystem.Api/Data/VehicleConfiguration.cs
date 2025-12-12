using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourierManagementSystem.Api.Constants;

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
                .HasPrecision(AppConstants.maxweight_precision, AppConstants.maxweight_scale);

            builder.Property(v => v.MaxVolume)
                .HasPrecision(AppConstants.maxvolume_precision, AppConstants.maxvolume_scale);
        }
    }
}