using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanteFrance.Data;
using SanteFrance.Models;

namespace SanteFrance.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Vérifier si l'utilisateur est connecté
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // Récupérer l'utilisateur connecté
        private async Task<User?> GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return null;
            return await _context.Users.FindAsync(userId);
        }

        // GET: Student/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            var rendezVous = await _context.RendezVous
                .Include(r => r.Medecin)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.DateHeure)
                .Take(5)
                .ToListAsync();

            ViewBag.User = user;
            ViewBag.RendezVous = rendezVous;
            return View();
        }

        // GET: Student/Profil
        public async Task<IActionResult> Profil()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            var nombreRdv = await _context.RendezVous.CountAsync(r => r.UserId == user.Id);
            ViewBag.NombreRdv = nombreRdv;
            return View(user);
        }

        // POST: Student/UpdateProfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfil(User model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            user.Nom = model.Nom;
            user.Prenom = model.Prenom;
            user.Telephone = model.Telephone;
            user.Adresse = model.Adresse;
            user.CodePostal = model.CodePostal;
            user.Ville = model.Ville;
            user.Universite = model.Universite;
            user.NumeroSecu = model.NumeroSecu;
            user.ACarteVitale = model.ACarteVitale;

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profil mis à jour avec succès !";
            return RedirectToAction("Profil");
        }

        // POST: Student/UploadPhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] = "Veuillez sélectionner une photo.";
                return RedirectToAction("Profil");
            }

            // Vérifier le type de fichier
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] = "Format non supporté. Utilisez JPG, PNG, GIF ou WebP.";
                return RedirectToAction("Profil");
            }

            // Vérifier la taille (max 5 Mo)
            if (photo.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "La photo ne doit pas dépasser 5 Mo.";
                return RedirectToAction("Profil");
            }

            // Créer le dossier uploads s'il n'existe pas
            var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "photos");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            // Supprimer l'ancienne photo si elle existe
            if (!string.IsNullOrEmpty(user.PhotoUrl))
            {
                var oldPath = Path.Combine(_environment.WebRootPath, user.PhotoUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            // Générer un nom unique
            var fileName = $"user_{user.Id}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            // Sauvegarder le chemin en base
            user.PhotoUrl = $"/uploads/photos/{fileName}";
            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Photo de profil mise à jour !";
            return RedirectToAction("Profil");
        }

        // POST: Student/SupprimerPhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerPhoto()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (!string.IsNullOrEmpty(user.PhotoUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, user.PhotoUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                user.PhotoUrl = null;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Photo supprimée.";
            return RedirectToAction("Profil");
        }

        // GET: Student/MesDemarches
        public async Task<IActionResult> MesDemarches()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            ViewBag.User = user;
            return View();
        }

        // GET: Student/MesRendezVous
        public async Task<IActionResult> MesRendezVous()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            var rendezVous = await _context.RendezVous
                .Include(r => r.Medecin)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.DateHeure)
                .ToListAsync();

            ViewBag.User = user;
            return View(rendezVous);
        }

        // POST: Student/AnnulerRendezVous
        [HttpPost]
        public async Task<IActionResult> AnnulerRendezVous(int id)
        {
            if (!IsLoggedIn())
                return Json(new { success = false, message = "Non connecté" });
            var user = await GetCurrentUser();
            if (user == null)
                return Json(new { success = false, message = "Utilisateur introuvable" });

            var rdv = await _context.RendezVous
                .Where(r => r.Id == id && r.UserId == user.Id)
                .FirstOrDefaultAsync();

            if (rdv == null)
                return Json(new { success = false, message = "Rendez-vous introuvable" });
            if (rdv.DateHeure < DateTime.Now)
                return Json(new { success = false, message = "Impossible d'annuler un rendez-vous passé" });
            if (rdv.Statut == "Annulé")
                return Json(new { success = false, message = "Ce rendez-vous est déjà annulé" });

            rdv.Statut = "Annulé";
            rdv.DateModification = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Rendez-vous annulé avec succès" });
        }

        // GET: Student/PrendreRendezVous
        public async Task<IActionResult> PrendreRendezVous(int medecinId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var medecin = await _context.Medecins.FindAsync(medecinId);
            if (medecin == null)
            {
                TempData["Error"] = "Médecin introuvable.";
                return RedirectToAction("Medecins", "Home");
            }
            ViewBag.Medecin = medecin;
            ViewBag.User = await GetCurrentUser();
            return View();
        }

        // POST: Student/PrendreRendezVous
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrendreRendezVous(int medecinId, string dateHeure, string typeRdv, string motif)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var user = await GetCurrentUser();
            if (user == null)
                return RedirectToAction("Login", "Account");

            DateTime rdvDateTime;
            if (!DateTime.TryParse(dateHeure, out rdvDateTime))
            {
                TempData["Error"] = "Format de date invalide.";
                return RedirectToAction("PrendreRendezVous", new { medecinId });
            }
            if (rdvDateTime < DateTime.Now)
            {
                TempData["Error"] = "La date du rendez-vous doit être dans le futur.";
                return RedirectToAction("PrendreRendezVous", new { medecinId });
            }

            var rdv = new RendezVous
            {
                UserId = user.Id,
                MedecinId = medecinId,
                DateHeure = rdvDateTime,
                DureeMinutes = 30,
                TypeRdv = typeRdv ?? "Sur place",
                Motif = motif,
                Statut = "En attente",
                DateCreation = DateTime.Now
            };

            _context.RendezVous.Add(rdv);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Rendez-vous créé avec succès !";
            return RedirectToAction("MesRendezVous");
        }

        // Retourne un TABLEAU (pas un objet) + dates en ISO
        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents(int medecinId)
        {
            if (!IsLoggedIn())
                return Json(Array.Empty<object>());

            var rdvs = await _context.RendezVous
                .Where(r => r.MedecinId == medecinId && r.Statut != "Annulé")
                .Select(r => new
                {
                    id = r.Id,
                    title = "Occupé",
                    start = r.DateHeure.ToString("o"),
                    end = r.DateHeure.AddMinutes(r.DureeMinutes).ToString("o"),
                    color = "#EF4444",
                    allDay = false
                })
                .ToListAsync();

            return Json(rdvs);
        }

        // GET: Student/CalendrierMedecin?medecinId=1
        public async Task<IActionResult> CalendrierMedecin(int medecinId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            var medecin = await _context.Medecins.FindAsync(medecinId);
            if (medecin == null)
            {
                TempData["Error"] = "Médecin introuvable.";
                return RedirectToAction("Medecins", "Home");
            }
            ViewBag.Medecin = medecin;
            ViewBag.User = await GetCurrentUser();
            return View();
        }

        // POST: Student/PrendreRendezVousCalendrier
        [HttpPost]
        public async Task<IActionResult> PrendreRendezVousCalendrier(int medecinId, string dateHeure, string typeRdv, string motif)
        {
            if (!IsLoggedIn())
                return Json(new { success = false, message = "Non connecté" });
            var user = await GetCurrentUser();
            if (user == null)
                return Json(new { success = false, message = "Utilisateur introuvable" });

            DateTime rdvDateTime;
            if (!DateTime.TryParse(dateHeure, out rdvDateTime))
                return Json(new { success = false, message = "Format de date invalide" });
            if (rdvDateTime < DateTime.Now)
                return Json(new { success = false, message = "Ce créneau est déjà passé" });

            // Médecin déjà pris ?
            var medecinOccupe = await _context.RendezVous
                .Where(r => r.MedecinId == medecinId && r.Statut != "Annulé")
                .Where(r => r.DateHeure == rdvDateTime)
                .AnyAsync();
            if (medecinOccupe)
                return Json(new { success = false, message = "Ce créneau est déjà réservé par une autre personne" });

            // Étudiant a déjà un RDV à cette heure ?
            var etudiantOccupe = await _context.RendezVous
                .Where(r => r.UserId == user.Id && r.Statut != "Annulé")
                .Where(r => r.DateHeure == rdvDateTime)
                .AnyAsync();
            if (etudiantOccupe)
                return Json(new { success = false, message = "Vous avez déjà un rendez-vous à cette heure" });

            var rdv = new RendezVous
            {
                UserId = user.Id,
                MedecinId = medecinId,
                DateHeure = rdvDateTime,
                DureeMinutes = 30,
                TypeRdv = typeRdv ?? "Sur place",
                Motif = motif,
                Statut = "En attente",
                DateCreation = DateTime.Now
            };

            _context.RendezVous.Add(rdv);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Rendez-vous créé avec succès" });
        }
    }
}