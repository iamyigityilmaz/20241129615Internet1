using HaberPortali2.Data;
using HaberPortali2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HaberPortali2.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------------------------
        // LOGIN (GET)
        // -----------------------------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // -----------------------------------------------------------
        // LOGIN (POST)
        // -----------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Şifreyi hashle
            string hashedPassword = HashPassword(password);

            // Kullanıcıyı bul
            var user = _context.Users
                .FirstOrDefault(x => x.Email == email && x.PasswordHash == hashedPassword);

            if (user == null)
            {
                ViewBag.Error = "E-Mail veya şifre hatalı!";
                return View();
            }

            // Cookie claim oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("IsAdmin", user.IsAdmin.ToString()) // Admin kontrolü için
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            // Admin ise admin paneline gönder
            if (user.IsAdmin)
                return RedirectToAction("Index", "News");

            // Normal kullanıcı ana sayfaya gider
            return RedirectToAction("Index", "Home");
        }

        // -----------------------------------------------------------
        // LOGOUT
        // -----------------------------------------------------------
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        // -----------------------------------------------------------
        // ŞİFRE HASH (SHA256)
        // -----------------------------------------------------------
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }
    }
}
