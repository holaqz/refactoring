using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierManagementSystem.Api.Models.Entities
{
    [Table("deliveries")]
    public class Delivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("courier_id")]
        [ForeignKey("Courier")]
        public long? CourierId { get; set; }
        public User? Courier { get; set; }

        [Column("vehicle_id")]
        [ForeignKey("Vehicle")]
        public long? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        [Column("created_by")]
        [Required]
        [ForeignKey("CreatedBy")]
        public long CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        [Column("delivery_date")]
        [Required]
        public DateOnly DeliveryDate { get; set; }

        [Column("time_start")]
        [Required]
        public TimeOnly TimeStart { get; set; }

        [Column("time_end")]
        [Required]
        public TimeOnly TimeEnd { get; set; }

        [Column("status")]
        [Required]
        public DeliveryStatus Status { get; set; } = DeliveryStatus.planned;

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<DeliveryPoint> DeliveryPoints { get; set; } = new List<DeliveryPoint>();
    }

    public enum DeliveryStatus
    {
        planned,
        in_progress,
        completed,
        cancelled
    }
}
