using KulupYonetimi.Data;
using KulupYonetimi.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
            var userId = GetCurrentUserId();
            var yoneticininKulubu = await _context.Kulupler.FirstOrDefaultAsync(k => k.YoneticiId == userId);

            if (yoneticininKulubu == null)
            {
                // Bu yöneticinin bir kulübü yok, duyuru oluşturamaz.
                return Unauthorized();
            }

            var duyuru = new Duyuru
            {
                KulupId = yoneticininKulubu.Id,
                GecerlilikBitis = DateTime.Today
            };

            return View(duyuru);
        }

        // POST: Duyuru/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Baslik,Icerik,KulupId,GecerlilikBitis,GecerlilikSuresiGun")] Duyuru duyuru)
        {
            var userId = GetCurrentUserId();
            var yoneticininKulubu = await _context.Kulupler.FirstOrDefaultAsync(k => k.YoneticiId == userId);

            if (yoneticininKulubu == null || duyuru.KulupId != yoneticininKulubu.Id)
            {
                return Unauthorized();
            }

            ModelState.Remove("Kulup"); // Navigation property'den kaynaklanan gereksiz hatayı kaldır.

            var today = DateTime.Today;
            DateTime? gecerlilikBitis = null;

            if (duyuru.GecerlilikBitis.HasValue)
            {
                if (duyuru.GecerlilikBitis.Value.Date < today)
                {
                    ModelState.AddModelError("GecerlilikBitis", "Geçmiş bir tarih seçilemez.");
                }
                else
                {
                    gecerlilikBitis = duyuru.GecerlilikBitis.Value.Date;
                }
            }

            if (duyuru.GecerlilikSuresiGun.HasValue)
            {
                if (duyuru.GecerlilikSuresiGun.Value <= 0)
                {
                    ModelState.AddModelError("GecerlilikSuresiGun", "Geçerli bir gün sayısı giriniz.");
                }
                else if (!gecerlilikBitis.HasValue)
                {
                    gecerlilikBitis = today.AddDays(duyuru.GecerlilikSuresiGun.Value);
                }
            }

            if (!gecerlilikBitis.HasValue)
            {
                ModelState.AddModelError("GecerlilikBitis", "Bir son tarih veya gün sayısı belirtmelisiniz.");
            }

            if (ModelState.IsValid)
            {
                duyuru.GecerlilikBitis = gecerlilikBitis;
                if (!duyuru.GecerlilikSuresiGun.HasValue && gecerlilikBitis.HasValue)
                {
                    var gunFarki = (int)Math.Ceiling((gecerlilikBitis.Value - today).TotalDays);
                    duyuru.GecerlilikSuresiGun = Math.Max(gunFarki, 1);
                }

                duyuru.YayinTarihi = DateTime.Now;
                _context.Add(duyuru);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Kulup", new { id = duyuru.KulupId });
            }
            return View(duyuru);
        }

        private int GetCurrentUserId()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idValue))
            {
                throw new InvalidOperationException("Kullanıcı bilgisi bulunamadı.");
            }

            return int.Parse(idValue);
        }
    }
}
