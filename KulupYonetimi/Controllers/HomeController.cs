using System;
using System.Diagnostics;
using System.Security.Claims;
using KulupYonetimi.Data;
using KulupYonetimi.Models;
using KulupYonetimi.Models.Entities;
using KulupYonetimi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KulupYonetimi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var viewModel = new HomeIndexViewModel();

            if (User.IsInRole("KulupYoneticisi"))
            {
                viewModel.IsYonetici = true;
                var yonetilenKulup = await _context.Kulupler
                    .Include(k => k.Duyurular)
                    .FirstOrDefaultAsync(k => k.YoneticiId == userId);

                var dashboard = new YoneticiDashboardViewModel
                {
                    YonetilenKulup = yonetilenKulup,
                    Duyurular = yonetilenKulup?.Duyurular
                        .OrderByDescending(d => d.YayinTarihi)
                        .ToList() ?? new List<Duyuru>()
                };

                if (yonetilenKulup != null)
                {
                    var kulupEtkinlikleri = await _context.Etkinlikler
                        .Where(e => e.KulupId == yonetilenKulup.Id)
                        .Include(e => e.Kayitlar)
                        .Include(e => e.Degerlendirmeler)
                        .OrderByDescending(e => e.Tarih)
                        .ToListAsync();

                    var simdi = DateTime.Now;

                    dashboard.Etkinlikler = kulupEtkinlikleri
                        .Where(e => e.Tarih >= simdi)
                        .OrderByDescending(e => e.Tarih)
                        .ToList();

                    dashboard.GecmisEtkinlikAnalizleri = kulupEtkinlikleri
                        .Where(e => e.Tarih < simdi)
                        .Select(e =>
                        {
                            var degerlendirmeler = e.Degerlendirmeler;
                            var ortalamaPuan = degerlendirmeler.Any()
                                ? Math.Round(degerlendirmeler.Average(d => d.Puan), 2)
                                : (double?)null;

                            return new EtkinlikAnalizViewModel
                            {
                                EtkinlikId = e.Id,
                                Ad = e.Ad,
                                Tarih = e.Tarih,
                                KatilimciSayisi = e.Kayitlar.Count,
                                DegerlendirmeSayisi = degerlendirmeler.Count,
                                YorumSayisi = degerlendirmeler.Count(d => !string.IsNullOrWhiteSpace(d.Yorum)),
                                OrtalamaPuan = ortalamaPuan
                            };
                        })
                        .OrderByDescending(a => a.Tarih)
                        .ToList();
                }

                viewModel.YoneticiDashboard = dashboard;
            }
            else // Öğrenci
            {
                viewModel.IsYonetici = false;
                var uyeOlunanKulupIdleri = await _context.KullaniciKulupler
                    .Where(kk => kk.KullaniciId == userId)
                    .Select(kk => kk.KulupId)
                    .ToListAsync();

                var etkinlikler = await _context.Etkinlikler
                    .Where(e => uyeOlunanKulupIdleri.Contains(e.KulupId))
                    .Include(e => e.Kulup)
                    .OrderByDescending(e => e.Tarih)
                    .ToListAsync();
                
                viewModel.OgrenciDashboard = new OgrenciDashboardViewModel { Etkinlikler = etkinlikler };
            }

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
