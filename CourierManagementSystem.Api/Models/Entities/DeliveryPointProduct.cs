using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierManagementSystem.Api.Models.Entities
{
    [Table("delivery_point_products")]
    public class DeliveryPointProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("delivery_point_id")]
        [Required]
        [ForeignKey("DeliveryPoint")]
        public long DeliveryPointId { get; set; }
        public DeliveryPoint DeliveryPoint { get; set; } = null!;

        [Column("product_id")]
        [Required]
        [ForeignKey("Product")]
        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }
    }
}
