using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Models.Entities
{
    [Table("delivery_points")]
    [Index(nameof(DeliveryId), nameof(Sequence), IsUnique = true)]
    public class DeliveryPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("delivery_id")]
        [Required]
        [ForeignKey("Delivery")]
        public long DeliveryId { get; set; }
        public Delivery Delivery { get; set; } = null!;

        [Column("sequence")]
        [Required]
        public int Sequence { get; set; }

        [Column("latitude", TypeName = "decimal(10,8)")]
        [Required]
        public decimal Latitude { get; set; }

        [Column("longitude", TypeName = "decimal(11,8)")]
        [Required]
        public decimal Longitude { get; set; }

        public ICollection<DeliveryPointProduct> DeliveryPointProducts { get; set; } = new List<DeliveryPointProduct>();
    }
}
