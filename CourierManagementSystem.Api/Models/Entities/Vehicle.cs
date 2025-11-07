using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierManagementSystem.Api.Models.Entities
{
    [Table("vehicles")]
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("brand")]
        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = null!;

        [Column("license_plate")]
        [Required]
        [MaxLength(20)]
        public string LicensePlate { get; set; } = null!;

        [Column("max_weight", TypeName = "decimal(8,2)")]
        [Required]
        public decimal MaxWeight { get; set; }

        [Column("max_volume", TypeName = "decimal(8,3)")]
        [Required]
        public decimal MaxVolume { get; set; }
    }
}
