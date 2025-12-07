using HaberPortali2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaberPortali2.Controllers
{
    public class NewsController : Controller
    {
        private readonly AppDbContext _context;

        public NewsController(AppDbContext context)
        {
            _context = context;
        }

        // ==============================
        // /News/Index  → tüm haberler
        // ==============================
        public async Task<IActionResult> Index()
        {
            var newsList = await _context.News
                .Include(n => n.Category)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(newsList);
        }

        // ==============================
        // /News/Detail/5 → haber detayı
        // ==============================
        public async Task<IActionResult> Detail(int id)
        {
            var news = await _context.News
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
                return NotFound();

            return View(news);
        }
    }
}
