using HaberPortali2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Haber sayısı
            ViewBag.TotalNews = await _context.News.CountAsync();

            // Artık taslak yayın durumu olmadığı için bu alanlar tamamen kaldırıldı
            // ViewBag.PublishedNews = ...  
            // ViewBag.DraftNews = ...

            // Kategori sayısı
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.ActiveCategories = await _context.Categories.CountAsync(x => x.IsActive == true);

            // Yorum sayısı
            ViewBag.TotalComments = await _context.Comments.CountAsync();

            return View();
        }
    }
}
