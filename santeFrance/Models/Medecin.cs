using System.ComponentModel.DataAnnotations;

namespace SanteFrance.Models
{
    public class Medecin
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
        [StringLength(150)]
        public string Specialite { get; set; }

        [Required]
        [StringLength(200)]
        public string Adresse { get; set; }

        [Required]
        [StringLength(100)]
        public string Ville { get; set; }

        [Required]
        [StringLength(10)]
        public string CodePostal { get; set; }

        [Phone]
        [StringLength(20)]
        public string? Telephone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Langues { get; set; } // Ex: "Français, Anglais, Arabe"

        public bool AccepteCarteVitale { get; set; } = true;

        public bool TiersPayant { get; set; } = false;

        [StringLength(500)]
        public string? PhotoUrl { get; set; } // URL de la photo ou emoji

        [StringLength(1000)]
        public string? Description { get; set; }

        // Horaires (format JSON ou texte)
        public string? Horaires { get; set; }

        public bool EstActif { get; set; } = true;

        public DateTime DateAjout { get; set; } = DateTime.Now;

        // Relations
        public virtual ICollection<RendezVous>? RendezVous { get; set; }
    }
}