using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourierManagementSystem.Api.Data.Configurations
{
    public class DeliveryPointProductConfiguration : IEntityTypeConfiguration<DeliveryPointProduct>
    {
        public void Configure(EntityTypeBuilder<DeliveryPointProduct> builder)
        {
            builder.ToTable("delivery_point_products");

            // Configure relationship with Product
            builder.HasOne(dpp => dpp.Product)
                .WithMany()
                .HasForeignKey(dpp => dpp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}