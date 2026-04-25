using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanteFrance.Data;
using SanteFrance.Models;
using BCrypt.Net;

namespace SanteFrance.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, string confirmerMotDePasse)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Cet email est déjà utilisé.");
                    return View(user);
                }

                if (user.MotDePasse != confirmerMotDePasse)
                {
                    ModelState.AddModelError("MotDePasse", "Les mots de passe ne correspondent pas.");
                    return View(user);
                }

                user.MotDePasse = BCrypt.Net.BCrypt.HashPassword(user.MotDePasse);
                user.DateInscription = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Inscription réussie ! Vous pouvez maintenant vous connecter.";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string motDePasse, string userType)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(motDePasse))
            {
                ViewBag.Error = "Veuillez remplir tous les champs.";
                return View();
            }

            // ✅ Connexion Admin
            if (userType == "Admin")
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
                if (admin != null && admin.EstActif && BCrypt.Net.BCrypt.Verify(motDePasse, admin.MotDePasse))
                {
                    HttpContext.Session.SetInt32("AdminId", admin.Id);
                    HttpContext.Session.SetString("AdminName", admin.Prenom + " " + admin.Nom);
                    HttpContext.Session.SetString("AdminEmail", admin.Email);
                    HttpContext.Session.SetString("UserType", "Admin");

                    admin.DerniereConnexion = DateTime.Now;
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Connexion admin réussie !";
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            // ✅ Connexion Médecin (AMÉLIORATION avec messages détaillés)
            else if (userType == "Medecin")
            {
                var medecin = await _context.Medecins
                    .FirstOrDefaultAsync(m => m.Email.ToLower() == email.ToLower());
                
                if (medecin == null)
                {
                    ViewBag.Error = $"❌ Aucun médecin trouvé avec l'email : {email}";
                    return View();
                }

                if (string.IsNullOrEmpty(medecin.MotDePasse))
                {
                    ViewBag.Error = "❌ Le mot de passe du médecin n'est pas défini. Contactez l'administrateur.";
                    return View();
                }

                if (!medecin.EstActif)
                {
                    ViewBag.Error = "❌ Votre compte a été désactivé. Contactez l'administrateur.";
                    return View();
                }

                try
                {
                    bool passwordVerified = BCrypt.Net.BCrypt.Verify(motDePasse, medecin.MotDePasse);
                    
                    if (passwordVerified)
                    {
                        HttpContext.Session.SetInt32("MedecinId", medecin.Id);
                        HttpContext.Session.SetString("MedecinName", $"Dr. {medecin.Prenom} {medecin.Nom}");
                        HttpContext.Session.SetString("MedecinEmail", medecin.Email);
                        HttpContext.Session.SetString("UserType", "Medecin");

                        medecin.DerniereConnexion = DateTime.Now;
                        await _context.SaveChangesAsync();

                        TempData["Success"] = $"Bienvenue Dr. {medecin.Prenom} {medecin.Nom} !";
                        return RedirectToAction("Dashboard", "Medecin");
                    }
                    else
                    {
                        ViewBag.Error = "❌ Mot de passe incorrect. Vérifiez vos identifiants.";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"❌ Erreur lors de la vérification : {ex.Message}";
                    return View();
                }
            }
            // ✅ Connexion Étudiant
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null && BCrypt.Net.BCrypt.Verify(motDePasse, user.MotDePasse))
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.Prenom + " " + user.Nom);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserType", "Etudiant");

                    TempData["Success"] = "Connexion réussie ! Bienvenue " + user.Prenom + " !";
                    return RedirectToAction("Dashboard", "Student");
                }
            }

            ViewBag.Error = "Email ou mot de passe incorrect.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Vous êtes déconnecté.";
            return RedirectToAction("Index", "Home");
        }
    }
}