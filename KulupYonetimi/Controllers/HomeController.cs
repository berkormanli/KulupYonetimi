using System.Diagnostics;
using System.Security.Claims;
using KulupYonetimi.Data;
using KulupYonetimi.Models;
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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var viewModel = new HomeIndexViewModel();

            if (User.IsInRole("KulupYoneticisi"))
            {
                viewModel.IsYonetici = true;
                var yonetilenKulup = await _context.Kulupler
                    .Include(k => k.Etkinlikler)
                    .Include(k => k.Duyurular)
                    .FirstOrDefaultAsync(k => k.YoneticiId == userId);

                viewModel.YoneticiDashboard = new YoneticiDashboardViewModel
                {
                    YonetilenKulup = yonetilenKulup,
                    Etkinlikler = yonetilenKulup?.Etkinlikler.OrderByDescending(e => e.Tarih).ToList() ?? new List<Models.Entities.Etkinlik>(),
                    Duyurular = yonetilenKulup?.Duyurular.OrderByDescending(d => d.YayinTarihi).ToList() ?? new List<Models.Entities.Duyuru>()
                };
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
    }
}
