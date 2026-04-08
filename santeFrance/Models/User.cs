using System.ComponentModel.DataAnnotations;

namespace SanteFrance.Models
{
    public class User
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

        [Phone]
        public string? Telephone { get; set; }

        [Required]
        public DateTime DateNaissance { get; set; }

        [Required]
        [StringLength(100)]
        public string Nationalite { get; set; }

        [StringLength(200)]
        public string? Universite { get; set; }

        [StringLength(20)]
        public string? NumeroSecu { get; set; }

        public bool ACarteVitale { get; set; } = false;

        [StringLength(200)]
        public string? Adresse { get; set; }

        [StringLength(10)]
        public string? CodePostal { get; set; }

        [StringLength(100)]
        public string? Ville { get; set; }

        [StringLength(500)]
        public string? PhotoUrl { get; set; } // Chemin de la photo de profil

        public DateTime DateInscription { get; set; } = DateTime.Now;

        // Relations
        public virtual ICollection<RendezVous>? RendezVous { get; set; }
    }
}