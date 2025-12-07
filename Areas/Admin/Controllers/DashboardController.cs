using HaberPortali2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        //  ADMIN ANA SAYFASI
        // ============================================================
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalNews = await _context.News.CountAsync();                   // Tüm haberler
            ViewBag.TotalCategories = await _context.Categories.CountAsync();       // Tüm kategoriler
            ViewBag.TotalComments = await _context.Comments.CountAsync();           // Tüm yorumlar

            return View();
        }
    }
}
