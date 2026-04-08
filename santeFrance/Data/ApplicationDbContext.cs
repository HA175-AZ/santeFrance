using Microsoft.EntityFrameworkCore;
using SanteFrance.Models;

namespace SanteFrance.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Medecin> Medecins { get; set; }
        public DbSet<RendezVous> RendezVous { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration par défaut pour TypeRdv
            modelBuilder.Entity<RendezVous>()
                .Property(r => r.TypeRdv)
                .HasDefaultValue("Sur place");

            // Configuration des relations
            modelBuilder.Entity<RendezVous>()
                .HasOne(r => r.User)
                .WithMany(u => u.RendezVous)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RendezVous>()
                .HasOne(r => r.Medecin)
                .WithMany(m => m.RendezVous)
                .HasForeignKey(r => r.MedecinId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index pour améliorer les performances
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Medecin>()
                .HasIndex(m => m.Ville);
        }
    }
}