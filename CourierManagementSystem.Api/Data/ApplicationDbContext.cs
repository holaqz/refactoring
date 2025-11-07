using System;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourierManagementSystem.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Vehicle> Vehicles { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Delivery> Deliveries { get; set; } = null!;
        public DbSet<DeliveryPoint> DeliveryPoints { get; set; } = null!;
        public DbSet<DeliveryPointProduct> DeliveryPointProducts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                
                entity.ToTable("users");

                entity.HasIndex(u => u.Login)
                    .IsUnique();

                entity.Property(u => u.Role)
                    .HasConversion<string>();
            });

            // Configure Vehicle entity
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicles");

                entity.HasIndex(v => v.LicensePlate)
                    .IsUnique();

                entity.Property(v => v.MaxWeight)
                    .HasPrecision(8, 2);

                entity.Property(v => v.MaxVolume)
                    .HasPrecision(8, 3);
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(p => p.Weight)
                    .HasPrecision(8, 3);

                entity.Property(p => p.Length)
                    .HasPrecision(6, 2);

                entity.Property(p => p.Width)
                    .HasPrecision(6, 2);

                entity.Property(p => p.Height)
                    .HasPrecision(6, 2);
            });

            // Configure Delivery entity
            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.ToTable("deliveries");

                entity.Property(d => d.Status)
                    .HasConversion<string>();

                // Configure relationship with Courier (User)
                entity.HasOne(d => d.Courier)
                    .WithMany()
                    .HasForeignKey(d => d.CourierId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Configure relationship with Vehicle
                entity.HasOne(d => d.Vehicle)
                    .WithMany()
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Configure relationship with CreatedBy (User)
                entity.HasOne(d => d.CreatedBy)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure relationship with DeliveryPoints
                entity.HasMany(d => d.DeliveryPoints)
                    .WithOne(dp => dp.Delivery)
                    .HasForeignKey(dp => dp.DeliveryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DeliveryPoint entity
            modelBuilder.Entity<DeliveryPoint>(entity =>
            {
                entity.ToTable("delivery_points");

                // Unique constraint on DeliveryId and Sequence
                entity.HasIndex(dp => new { dp.DeliveryId, dp.Sequence })
                    .IsUnique();

                entity.Property(dp => dp.Latitude)
                    .HasPrecision(10, 8);

                entity.Property(dp => dp.Longitude)
                    .HasPrecision(11, 8);

                // Configure relationship with DeliveryPointProducts
                entity.HasMany(dp => dp.DeliveryPointProducts)
                    .WithOne(dpp => dpp.DeliveryPoint)
                    .HasForeignKey(dpp => dpp.DeliveryPointId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DeliveryPointProduct entity
            modelBuilder.Entity<DeliveryPointProduct>(entity =>
            {
                entity.ToTable("delivery_point_products");

                // Configure relationship with Product
                entity.HasOne(dpp => dpp.Product)
                    .WithMany()
                    .HasForeignKey(dpp => dpp.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data for admin user
            modelBuilder.Entity<User>().HasData(
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
