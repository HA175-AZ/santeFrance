using System.ComponentModel.DataAnnotations;

namespace SanteFrance.Models
{
    public class Medecin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Specialite { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Adresse { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Ville { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string CodePostal { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Telephone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasse { get; set; } = string.Empty;

        [Required]
        public string Langues { get; set; } = string.Empty;

        public bool AccepteCarteVitale { get; set; } = true;

        public bool TiersPayant { get; set; } = false;

        [StringLength(500)]
        public string? PhotoUrl { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public string? Horaires { get; set; }

        public bool EstActif { get; set; } = true;

        public DateTime DateAjout { get; set; } = DateTime.Now;

        public DateTime? DerniereConnexion { get; set; }

        public virtual ICollection<RendezVous>? RendezVous { get; set; }
    }
}