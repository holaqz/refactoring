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
                .HasPrecision(DbConstatnts.maxweight_precision, DbConstatnts.maxweight_scale);

            builder.Property(v => v.MaxVolume)
                .HasPrecision(DbConstatnts.maxvolume_precision, DbConstatnts.maxvolume_scale);
        }
    }
}