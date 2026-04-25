using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanteFrance.Data;
using SanteFrance.Models;

namespace SanteFrance.Controllers
{
    public class MedecinController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedecinController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsMedecinLoggedIn()
        {
            return HttpContext.Session.GetInt32("MedecinId") != null;
        }

        private async Task<Medecin?> GetCurrentMedecin()
        {
            var medecinId = HttpContext.Session.GetInt32("MedecinId");
            if (medecinId == null) return null;
            return await _context.Medecins.FindAsync(medecinId);
        }

        // GET: Medecin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsMedecinLoggedIn())
                return RedirectToAction("Login", "Account");

            var medecin = await GetCurrentMedecin();
            if (medecin == null)
                return RedirectToAction("Login", "Account");

            var stats = new
            {
                RdvAujourdhui = await _context.RendezVous
                    .Where(r => r.MedecinId == medecin.Id && r.DateHeure.Date == DateTime.Today && r.Statut != "Annulé")
                    .CountAsync(),
                RdvEnAttente = await _context.RendezVous
                    .Where(r => r.MedecinId == medecin.Id && r.Statut == "En attente")
                    .CountAsync(),
                RdvSemaine = await _context.RendezVous
                    .Where(r => r.MedecinId == medecin.Id && r.DateHeure >= DateTime.Today && r.DateHeure <= DateTime.Today.AddDays(7) && r.Statut != "Annulé")
                    .CountAsync(),
                TotalPatients = await _context.RendezVous
                    .Where(r => r.MedecinId == medecin.Id)
                    .Select(r => r.UserId)
                    .Distinct()
                    .CountAsync()
            };

            ViewBag.Stats = stats;
            ViewBag.Medecin = medecin;
            return View();
        }

        // GET: Medecin/MesRendezVous
        public async Task<IActionResult> MesRendezVous()
        {
            if (!IsMedecinLoggedIn())
                return RedirectToAction("Login", "Account");

            var medecin = await GetCurrentMedecin();
            if (medecin == null)
                return RedirectToAction("Login", "Account");

            var rdvs = await _context.RendezVous
                .Include(r => r.User)
                .Where(r => r.MedecinId == medecin.Id)
                .OrderBy(r => r.DateHeure)
                .ToListAsync();

            ViewBag.Medecin = medecin;
            return View(rdvs);
        }

        // POST: Medecin/ChangerStatutRdv
        [HttpPost]
        public async Task<IActionResult> ChangerStatutRdv(int rdvId, string statut)
        {
            if (!IsMedecinLoggedIn())
                return Json(new { success = false, message = "Non connecté" });

            var medecin = await GetCurrentMedecin();
            if (medecin == null)
                return Json(new { success = false, message = "Médecin introuvable" });

            var rdv = await _context.RendezVous
                .Where(r => r.Id == rdvId && r.MedecinId == medecin.Id)
                .FirstOrDefaultAsync();

            if (rdv == null)
                return Json(new { success = false, message = "Rendez-vous introuvable" });

            rdv.Statut = statut;
            rdv.DateModification = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Statut mis à jour" });
        }

        // GET: Medecin/MonProfil
        public async Task<IActionResult> MonProfil()
        {
            if (!IsMedecinLoggedIn())
                return RedirectToAction("Login", "Account");

            var medecin = await GetCurrentMedecin();
            if (medecin == null)
                return RedirectToAction("Login", "Account");

            return View(medecin);
        }

        // POST: Medecin/UpdateProfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfil(Medecin model)
        {
            if (!IsMedecinLoggedIn())
                return RedirectToAction("Login", "Account");

            var medecin = await GetCurrentMedecin();
            if (medecin == null)
                return RedirectToAction("Login", "Account");

            medecin.Telephone = model.Telephone;
            medecin.Email = model.Email;
            medecin.Description = model.Description;
            medecin.Horaires = model.Horaires;

            _context.Update(medecin);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil mis à jour avec succès !";
            return RedirectToAction("MonProfil");
        }


    }
}