using KulupYonetimi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KulupYonetimi.Controllers
{
    [Authorize]
    public class KulupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KulupController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Kulup
        public async Task<IActionResult> Index()
        {
            var kulupler = await _context.Kulupler.ToListAsync();
            return View(kulupler);
        }

        // GET: Kulup/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kulup = await _context.Kulupler
                .Include(k => k.Etkinlikler)
                .Include(k => k.Duyurular)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (kulup == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.IsUye = await _context.KullaniciKulupler.AnyAsync(kk => kk.KulupId == id && kk.KullaniciId == userId);

            return View(kulup);
        }


        // GET: Kulup/Create
        [Authorize(Roles = "KulupYoneticisi")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kulup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "KulupYoneticisi")]
        public async Task<IActionResult> Create([Bind("Ad,Aciklama")] KulupYonetimi.Models.Entities.Kulup kulup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kulup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kulup);
        }

        // POST: Kulup/Join/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ogrenci")]
        public async Task<IActionResult> Join(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var isUye = await _context.KullaniciKulupler.AnyAsync(kk => kk.KulupId == id && kk.KullaniciId == userId);
            if (!isUye)
            {
                var kullaniciKulup = new KulupYonetimi.Models.Entities.KullaniciKulup { KulupId = id, KullaniciId = userId };
                _context.KullaniciKulupler.Add(kullaniciKulup);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
