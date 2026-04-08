using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanteFrance.Data;
using SanteFrance.Models;

namespace SanteFrance.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetInt32("AdminId") != null;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var stats = new
            {
                TotalMedecins = await _context.Medecins.CountAsync(),
                TotalEtudiants = await _context.Users.CountAsync(),
                TotalRdv = await _context.RendezVous.CountAsync(),
                RdvEnAttente = await _context.RendezVous.CountAsync(r => r.Statut == "En attente")
            };

            ViewBag.Stats = stats;
            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
            return View();
        }

        // GET: Admin/Medecins
        public async Task<IActionResult> Medecins()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var medecins = await _context.Medecins
                .OrderByDescending(m => m.DateAjout)
                .ToListAsync();
            return View(medecins);
        }

        // GET: Admin/AjouterMedecin
        public IActionResult AjouterMedecin()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: Admin/AjouterMedecin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterMedecin(Medecin medecin, IFormFile? photo)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                medecin.DateAjout = DateTime.Now;
                medecin.EstActif = true;

                // Upload photo si fournie
                if (photo != null && photo.Length > 0)
                {
                    medecin.PhotoUrl = await SaveMedecinPhoto(photo);
                }

                _context.Medecins.Add(medecin);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Médecin ajouté avec succès !";
                return RedirectToAction("Medecins");
            }
            return View(medecin);
        }

        // GET: Admin/ModifierMedecin/5
        public async Task<IActionResult> ModifierMedecin(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var medecin = await _context.Medecins.FindAsync(id);
            if (medecin == null)
                return NotFound();
            return View(medecin);
        }

        // POST: Admin/ModifierMedecin/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierMedecin(int id, Medecin medecin, IFormFile? photo)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            if (id != medecin.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Récupérer l'ancien médecin pour garder la photo si pas de nouvelle
                    var existingMedecin = await _context.Medecins.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (photo != null && photo.Length > 0)
                    {
                        // Supprimer l'ancienne photo
                        if (existingMedecin != null && !string.IsNullOrEmpty(existingMedecin.PhotoUrl)
                            && existingMedecin.PhotoUrl.StartsWith("/uploads/"))
                        {
                            DeletePhotoFile(existingMedecin.PhotoUrl);
                        }
                        medecin.PhotoUrl = await SaveMedecinPhoto(photo);
                    }
                    else if (string.IsNullOrEmpty(medecin.PhotoUrl))
                    {
                        // Garder l'ancienne photo si le champ est vide
                        medecin.PhotoUrl = existingMedecin?.PhotoUrl;
                    }

                    _context.Update(medecin);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Médecin modifié avec succès !";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Medecins.Any(m => m.Id == medecin.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction("Medecins");
            }
            return View(medecin);
        }

        // POST: Admin/SupprimerPhotoMedecin
        [HttpPost]
        public async Task<IActionResult> SupprimerPhotoMedecin(int id)
        {
            if (!IsAdminLoggedIn())
                return Json(new { success = false, message = "Non autorisé" });

            var medecin = await _context.Medecins.FindAsync(id);
            if (medecin == null)
                return Json(new { success = false, message = "Médecin introuvable" });

            if (!string.IsNullOrEmpty(medecin.PhotoUrl) && medecin.PhotoUrl.StartsWith("/uploads/"))
            {
                DeletePhotoFile(medecin.PhotoUrl);
            }

            medecin.PhotoUrl = null;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Photo supprimée" });
        }

        // POST: Admin/SupprimerMedecin/5
        [HttpPost]
        public async Task<IActionResult> SupprimerMedecin(int id)
        {
            if (!IsAdminLoggedIn())
                return Json(new { success = false, message = "Non autorisé" });

            try
            {
                var medecin = await _context.Medecins.FindAsync(id);
                if (medecin == null)
                    return Json(new { success = false, message = "Médecin introuvable" });

                // Supprimer la photo du disque
                if (!string.IsNullOrEmpty(medecin.PhotoUrl) && medecin.PhotoUrl.StartsWith("/uploads/"))
                {
                    DeletePhotoFile(medecin.PhotoUrl);
                }

                var rdvs = await _context.RendezVous.Where(r => r.MedecinId == id).ToListAsync();
                if (rdvs.Any())
                    _context.RendezVous.RemoveRange(rdvs);

                _context.Medecins.Remove(medecin);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Médecin supprimé" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erreur : " + ex.Message });
            }
        }

        // GET: Admin/Etudiants
        public async Task<IActionResult> Etudiants()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var etudiants = await _context.Users.OrderByDescending(u => u.DateInscription).ToListAsync();
            var rdvParEtudiant = new Dictionary<int, int>();
            foreach (var etudiant in etudiants)
            {
                rdvParEtudiant[etudiant.Id] = await _context.RendezVous.CountAsync(r => r.UserId == etudiant.Id);
            }
            ViewBag.RdvParEtudiant = rdvParEtudiant;
            ViewBag.TotalRdv = await _context.RendezVous.CountAsync();
            return View(etudiants);
        }

        // GET: Admin/RendezVous
        public async Task<IActionResult> RendezVous()
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Account");

            var rdv = await _context.RendezVous
                .Include(r => r.User)
                .Include(r => r.Medecin)
                .OrderByDescending(r => r.DateHeure)
                .ToListAsync();
            return View(rdv);
        }

        // POST: Admin/ChangerStatutRdv
        [HttpPost]
        public async Task<IActionResult> ChangerStatutRdv([FromBody] ChangerStatutRequest request)
        {
            try
            {
                if (!IsAdminLoggedIn())
                    return Json(new { success = false, message = "Non autorisé" });

                var rdv = await _context.RendezVous.FindAsync(request.RdvId);
                if (rdv == null)
                    return Json(new { success = false, message = "RDV introuvable" });

                rdv.Statut = request.Statut;
                rdv.DateModification = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Statut mis à jour" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erreur: " + ex.Message });
            }
        }

        // === Méthodes privées pour les photos ===
        private async Task<string> SaveMedecinPhoto(IFormFile photo)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Format non supporté");

            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "medecins");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"med_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return $"/uploads/medecins/{fileName}";
        }

        private void DeletePhotoFile(string photoUrl)
        {
            var filePath = Path.Combine(_environment.WebRootPath, photoUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        public class ChangerStatutRequest
        {
            public int RdvId { get; set; }
            public string Statut { get; set; } = "";
        }
    }
}