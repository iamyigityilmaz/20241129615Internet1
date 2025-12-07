using HaberPortali2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaberPortali2.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // ================================
        // ANA SAYFA → Son haberler
        // ================================
        public async Task<IActionResult> Index()
        {
            // Haberler
            var newsList = await _context.News
                .Include(n => n.Category)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            // Navbar için aktif kategoriler
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return View(newsList);
        }

        // ================================
        // KATEGORİYE GÖRE HABERLER
        // ================================
        public async Task<IActionResult> Category(int id)
        {
            var newsList = await _context.News
                .Include(n => n.Category)
                .Where(n => n.CategoryId == id)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            var category = await _context.Categories.FindAsync(id);

            ViewBag.CategoryName = category?.Name ?? "Kategori";

            // Navbar için kategoriler (tekrar yüklenmeli)
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return View("Index", newsList);
        }
    }
}
