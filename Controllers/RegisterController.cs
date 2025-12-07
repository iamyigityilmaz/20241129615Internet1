using HaberPortali2.Data;
using HaberPortali2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace HaberPortali2.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // KAYIT SAYFASI (GET)
        // =======================================================
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // =======================================================
        // KAYIT OL (POST)
        // =======================================================
        [HttpPost]
        public IActionResult Index(string fullName, string email, string password)
        {
            // Email zaten var mı kontrol et
            var exists = _context.Users.FirstOrDefault(x => x.Email == email);

            if (exists != null)
            {
                ViewBag.Error = "Bu e-mail adresi zaten kayıtlı!";
                return View();
            }

            // Şifreyi hashle
            string hashed = HashPassword(password);

            // Yeni kullanıcı oluştur
            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = hashed,
                CreatedDate = DateTime.Now,
                IsAdmin = false // varsayılan olarak admin değil
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            ViewBag.Success = "Kayıt başarılı! Giriş yapabilirsiniz.";

            return View();
        }

        // =======================================================
        // Şifre Hashleme (SHA256)
        // =======================================================
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
