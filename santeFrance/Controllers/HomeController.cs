using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanteFrance.Data;
using SanteFrance.Models;
using System.Diagnostics;

namespace SanteFrance.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ComprendreLaSante()
        {
            ViewData["Title"] = "Comprendre la santé";
            return View();
        }

        public async Task<IActionResult> Medecins(string search, string ville, string langue, string tiersPayant)
        {
            ViewData["Title"] = "Trouver un médecin";

            var query = _context.Medecins.Where(m => m.EstActif);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.Nom.Contains(search) ||
                    m.Prenom.Contains(search) ||
                    m.Specialite.Contains(search));
                ViewBag.Search = search;
            }

            if (!string.IsNullOrEmpty(ville))
            {
                query = query.Where(m =>
                    m.Ville.Contains(ville) ||
                    m.CodePostal.Contains(ville));
                ViewBag.Ville = ville;
            }

            if (!string.IsNullOrEmpty(langue))
            {
                query = query.Where(m => m.Langues.Contains(langue));
                ViewBag.Langue = langue;
            }

            if (tiersPayant == "true")
            {
                query = query.Where(m => m.TiersPayant);
                ViewBag.TiersPayant = tiersPayant;
            }

            var medecins = await query.OrderBy(m => m.Ville).ToListAsync();
            return View(medecins);
        }

        public IActionResult SecuriteSociale()
        {
            ViewData["Title"] = "Sécurité sociale";
            return View();
        }

        public IActionResult Urgences()
        {
            ViewData["Title"] = "Urgences";
            return View();
        }

        public IActionResult SanteMentale()
        {
            ViewData["Title"] = "Santé mentale";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}