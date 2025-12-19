using KulupYonetimi.Data;
using KulupYonetimi.Models.Entities;
using KulupYonetimi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace KulupYonetimi.Controllers
{
    [Authorize]
    public class EtkinlikController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EtkinlikController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Etkinlik
        public async Task<IActionResult> Index()
        {
            var allEvents = await _context.Etkinlikler.Include(e => e.Kulup).ToListAsync();

            if (User.IsInRole("Ogrenci"))
            {
                var userId = GetCurrentUserId();
                var uyeOlunanKulupIdleri = await _context.KullaniciKulupler
                    .Where(kk => kk.KullaniciId == userId)
                    .Select(kk => kk.KulupId)
                    .ToListAsync();

                var sortedEvents = allEvents
                    .OrderByDescending(e => uyeOlunanKulupIdleri.Contains(e.KulupId))
                    .ThenByDescending(e => e.Tarih)
                    .ToList();
                
                return View(sortedEvents);
            }

            return View(allEvents.OrderByDescending(e => e.Tarih).ToList());
        }

        // GET: Etkinlik/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var etkinlik = await _context.Etkinlikler
                .Include(e => e.Kulup)
                .Include(e => e.Degerlendirmeler)
                .ThenInclude(d => d.Kullanici)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (etkinlik == null) return NotFound();

            var userId = GetCurrentUserId();
            ViewBag.IsKayitli = await _context.EtkinlikKayitlari.AnyAsync(ek => ek.EtkinlikId == id && ek.KullaniciId == userId);
            ViewBag.HasDegerlendirme = await _context.Degerlendirmeler.AnyAsync(d => d.EtkinlikId == id && d.KullaniciId == userId);

            return View(etkinlik);
        }

        // GET: Etkinlik/Create
        [Authorize(Roles = "KulupYoneticisi")]
        public IActionResult Create()
        {
            var times = new List<string>();
            for (var i = 0; i < 24; i++)
            {
                times.Add(i.ToString("00") + ":00");
                times.Add(i.ToString("00") + ":15");
                times.Add(i.ToString("00") + ":30");
                times.Add(i.ToString("00") + ":45");
            }
            ViewBag.Times = times;

            var viewModel = new EtkinlikCreateViewModel();
            return View(viewModel);
        }

        // POST: Etkinlik/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "KulupYoneticisi")]
        public async Task<IActionResult> Create(EtkinlikCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var yoneticininKulubu = await _context.Kulupler.FirstOrDefaultAsync(k => k.YoneticiId == userId);

                if (yoneticininKulubu == null) return Unauthorized();

                var saatDakika = viewModel.EtkinlikSaati.Split(':');
                var saat = int.Parse(saatDakika[0]);
                var dakika = int.Parse(saatDakika[1]);

                var etkinlikTarihi = new DateTime(viewModel.EtkinlikGunu.Year, viewModel.EtkinlikGunu.Month, viewModel.EtkinlikGunu.Day, saat, dakika, 0);

                if (etkinlikTarihi < DateTime.Now)
                {
                    ModelState.AddModelError("EtkinlikGunu", "Geçmiş bir tarihe etkinlik oluşturulamaz.");
                    var timesList = new List<string>();
                    for (var i = 0; i < 24; i++)
                    {
                        timesList.Add(i.ToString("00") + ":00");
                        timesList.Add(i.ToString("00") + ":15");
                        timesList.Add(i.ToString("00") + ":30");
                        timesList.Add(i.ToString("00") + ":45");
                    }
                    ViewBag.Times = timesList;
                    return View(viewModel);
                }

                var etkinlik = new Etkinlik
                {
                    Ad = viewModel.Ad,
                    Aciklama = viewModel.Aciklama,
                    Konum = viewModel.Konum,
                    Tarih = etkinlikTarihi,
                    KulupId = yoneticininKulubu.Id
                };

                _context.Add(etkinlik);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            var times = new List<string>();
            for (var i = 0; i < 24; i++)
            {
                times.Add(i.ToString("00") + ":00");
                times.Add(i.ToString("00") + ":15");
                times.Add(i.ToString("00") + ":30");
                times.Add(i.ToString("00") + ":45");
            }
            ViewBag.Times = times;
            return View(viewModel);
        }

        // Other actions...
        // POST: Etkinlik/Register/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ogrenci")]
        public async Task<IActionResult> Register(int id)
        {
            var userId = GetCurrentUserId();
            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik == null) return NotFound();

            if (etkinlik.Tarih <= DateTime.Now)
            {
                TempData["EtkinlikUyari"] = "Geçmiş etkinliklere kayıt yapılamaz.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var isKayitli = await _context.EtkinlikKayitlari.AnyAsync(ek => ek.EtkinlikId == id && ek.KullaniciId == userId);
            if (!isKayitli)
            {
                var kayit = new EtkinlikKayit { EtkinlikId = id, KullaniciId = userId };
                _context.EtkinlikKayitlari.Add(kayit);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Etkinlik/Degerlendir/5
        [Authorize(Roles = "Ogrenci")]
        public async Task<IActionResult> Degerlendir(int? id)
        {
            if (id == null) return NotFound();

            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik == null) return NotFound();

            var userId = GetCurrentUserId();
            var isKayitli = await _context.EtkinlikKayitlari.AnyAsync(ek => ek.EtkinlikId == id && ek.KullaniciId == userId);
            if (etkinlik.Tarih > DateTime.Now || !isKayitli) return RedirectToAction(nameof(Details), new { id });

            var mevcutDegerlendirme = await _context.Degerlendirmeler.FirstOrDefaultAsync(d => d.EtkinlikId == id && d.KullaniciId == userId);
            if(mevcutDegerlendirme != null)
            {
                TempData["DegerlendirmeMesaji"] = "Bu etkinliği zaten değerlendirdiniz.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.EtkinlikAdi = etkinlik.Ad;

            return View(new Degerlendirme { EtkinlikId = etkinlik.Id });
        }

        // POST: Etkinlik/Degerlendir/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ogrenci")]
        public async Task<IActionResult> Degerlendir(int id, [Bind("EtkinlikId,Puan,Yorum")] Degerlendirme degerlendirme)
        {
            if (id != degerlendirme.EtkinlikId) return NotFound();

            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik == null) return NotFound();

            ViewBag.EtkinlikAdi = etkinlik.Ad;

            var userId = GetCurrentUserId();
            var isKayitli = await _context.EtkinlikKayitlari.AnyAsync(ek => ek.EtkinlikId == id && ek.KullaniciId == userId);
            if (etkinlik.Tarih > DateTime.Now || !isKayitli)
            {
                TempData["DegerlendirmeMesaji"] = "Yalnızca katıldığınız ve tamamlanan etkinlikleri değerlendirebilirsiniz.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (ModelState.IsValid)
            {
                degerlendirme.KullaniciId = userId;
                degerlendirme.Tarih = DateTime.Now;

                var mevcutDegerlendirme = await _context.Degerlendirmeler.FirstOrDefaultAsync(d => d.EtkinlikId == id && d.KullaniciId == userId);

                if (mevcutDegerlendirme != null)
                {
                    TempData["DegerlendirmeMesaji"] = "Bu etkinliği zaten değerlendirdiniz.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                _context.Add(degerlendirme);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = degerlendirme.EtkinlikId });
            }

            var hataMesaji = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m)));

            TempData["DegerlendirmeMesaji"] = string.IsNullOrWhiteSpace(hataMesaji)
                ? "Değerlendirme kaydedilemedi. Lütfen tekrar deneyin."
                : hataMesaji;

            return RedirectToAction(nameof(Details), new { id });
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
