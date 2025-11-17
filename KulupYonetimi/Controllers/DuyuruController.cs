using KulupYonetimi.Data;
using KulupYonetimi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KulupYonetimi.Controllers
{
    [Authorize(Roles = "KulupYoneticisi")]
    public class DuyuruController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DuyuruController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Duyuru/Create
        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var yoneticininKulubu = await _context.Kulupler.FirstOrDefaultAsync(k => k.YoneticiId == userId);

            if (yoneticininKulubu == null)
            {
                // Bu yöneticinin bir kulübü yok, duyuru oluşturamaz.
                return Unauthorized();
            }

            var duyuru = new Duyuru
            {
                KulupId = yoneticininKulubu.Id
            };

            return View(duyuru);
        }

        // POST: Duyuru/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Baslik,Icerik,KulupId")] Duyuru duyuru)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var yoneticininKulubu = await _context.Kulupler.FirstOrDefaultAsync(k => k.YoneticiId == userId);

            if (yoneticininKulubu == null || duyuru.KulupId != yoneticininKulubu.Id)
            {
                return Unauthorized();
            }

            ModelState.Remove("Kulup"); // Navigation property'den kaynaklanan gereksiz hatayı kaldır.

            if (ModelState.IsValid)
            {
                _context.Add(duyuru);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Kulup", new { id = duyuru.KulupId });
            }
            return View(duyuru);
        }
    }
}
