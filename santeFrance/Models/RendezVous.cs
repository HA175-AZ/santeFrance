using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanteFrance.Models
{
    public class RendezVous
    {
        [Key]
        public int Id { get; set; }

        // Relations
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public int MedecinId { get; set; }

        [ForeignKey("MedecinId")]
        public virtual Medecin? Medecin { get; set; }

        // Informations du RDV
        [Required]
        public DateTime DateHeure { get; set; }

        // ⬇️ AJOUTER CETTE LIGNE ⬇️
        public int DureeMinutes { get; set; } = 30;

        [Required]
        [StringLength(20)]
        public string TypeRdv { get; set; } = "Sur place";

        [Required]
        [StringLength(50)]
        public string Statut { get; set; } = "En attente"; // En attente, Confirmé, Annulé, Terminé

        [StringLength(500)]
        public string? Motif { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public DateTime? DateModification { get; set; }
    }
}