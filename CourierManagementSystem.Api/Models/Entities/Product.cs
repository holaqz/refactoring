using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierManagementSystem.Api.Models.Entities
{
    [Table("products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = null!;

        [Column("weight", TypeName = "decimal(8,3)")]
        [Required]
        public decimal Weight { get; set; }

        [Column("length", TypeName = "decimal(6,2)")]
        [Required]
        public decimal Length { get; set; }

        [Column("width", TypeName = "decimal(6,2)")]
        [Required]
        public decimal Width { get; set; }

        [Column("height", TypeName = "decimal(6,2)")]
        [Required]
        public decimal Height { get; set; }

        public decimal GetVolume()
        {
            return Length * Width * Height / 1000000m; // convert cm³ to m³
        }
    }
}
