using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanteFrance.Data;
using SanteFrance.Models;

namespace SanteFrance.Controllers
{
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SetupController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Setup/CreateAdmin
        public async Task<IActionResult> CreateAdmin()
        {
            // Supprimer tous les anciens admins
            var oldAdmins = await _context.Admins.ToListAsync();
            if (oldAdmins.Any())
            {
                _context.Admins.RemoveRange(oldAdmins);
                await _context.SaveChangesAsync();
            }

            // Créer le nouveau compte admin avec BCrypt
            var admin = new Admin
            {
                Nom = "Admin",
                Prenom = "Super",
                Email = "admin@santefrance.fr",
                MotDePasse = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "SuperAdmin",
                EstActif = true,
                DateCreation = DateTime.Now
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return View();
        }
    }
}