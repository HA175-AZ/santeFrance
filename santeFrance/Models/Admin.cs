using System.ComponentModel.DataAnnotations;

namespace SanteFrance.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string MotDePasse { get; set; } // Sera hashé avec BCrypt

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Admin"; // Admin, SuperAdmin

        public bool EstActif { get; set; } = true;

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public DateTime? DerniereConnexion { get; set; }
    }
}