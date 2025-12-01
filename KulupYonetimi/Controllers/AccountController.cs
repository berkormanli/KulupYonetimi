using KulupYonetimi.Data;
using KulupYonetimi.Models.Entities;
using KulupYonetimi.Models.ViewModels;
using KulupYonetimi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KulupYonetimi.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AccountController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // Sadece yöneticisi olmayan kulüpleri listele
            ViewBag.Kulupler = new SelectList(await _context.Kulupler.Where(k => k.YoneticiId == null).ToListAsync(), "Id", "Ad");
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Rol == Rol.KulupYoneticisi && !model.KulupId.HasValue)
                {
                    ModelState.AddModelError("KulupId", "Kulüp Yöneticisi için bir kulüp seçmelisiniz.");
                    ViewBag.Kulupler = new SelectList(await _context.Kulupler.Where(k => k.YoneticiId == null).ToListAsync(), "Id", "Ad");
                    return View(model);
                }

                var existingUser = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Bu email adresi zaten kullanılıyor.");
                    ViewBag.Kulupler = new SelectList(await _context.Kulupler.Where(k => k.YoneticiId == null).ToListAsync(), "Id", "Ad");
                    return View(model);
                }

                var kullanici = new Kullanici
                {
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    Email = model.Email,
                    Rol = model.Rol,
                    Sifre = BCrypt.Net.BCrypt.HashPassword(model.Sifre)
                };

                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();

                if (model.Rol == Rol.KulupYoneticisi)
                {
                    var kulup = await _context.Kulupler.FindAsync(model.KulupId.Value);
                    if (kulup != null)
                    {
                        kulup.YoneticiId = kullanici.Id;
                        _context.Update(kulup);
                        await _context.SaveChangesAsync();
                    }
                }

                await _emailService.SendAsync(
                    kullanici.Email,
                    "Kulüp Yönetimi Kaydı",
                    "Kaydınız oluşturuldu. Doğrulama akışı eklendiğinde buraya gerekli bağlantılar gelecek."
                );

                return RedirectToAction("Login");
            }
            ViewBag.Kulupler = new SelectList(await _context.Kulupler.Where(k => k.YoneticiId == null).ToListAsync(), "Id", "Ad");
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string sifre, bool rememberMe = false)
        {
            var user = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(sifre, user.Sifre))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Ad + " " + user.Soyad),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Rol.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Geçersiz email veya şifre.");
            return View();
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
