using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierManagementSystem.Api.Models.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("login")]
        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = null!;

        [Column("password_hash")]
        [Required]
        public string PasswordHash { get; set; } = null!;

        [Column("name")]
        [Required]
        public string Name { get; set; } = null!;

        [Column("role")]
        [Required]
        public UserRole Role { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum UserRole
    {
        admin,
        manager,
        courier
    }
}
