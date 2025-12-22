using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IRepository<News> _newsRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Comment> _commentRepo;

        public DashboardController(
            IRepository<News> newsRepo,
            IRepository<Category> categoryRepo,
            IRepository<Comment> commentRepo)
        {
            _newsRepo = newsRepo;
            _categoryRepo = categoryRepo;
            _commentRepo = commentRepo;
        }

        public async Task<IActionResult> Index()
        {
            var allNews = await _newsRepo.GetAllAsync();
            var allCategories = await _categoryRepo.GetAllAsync();
            var allComments = await _commentRepo.GetAllAsync();

            ViewBag.TotalNews = allNews.Count();
            ViewBag.TotalCategories = allCategories.Count();
            ViewBag.TotalComments = allComments.Count();

            var today = DateTime.Today;
            ViewBag.TodayComments = allComments.Count(x => x.CreatedDate.Date == today);

            ViewBag.LastNews = allNews
                .OrderByDescending(x => x.Id)
                .Take(5)
                .ToList();

            ViewBag.LastComments = allComments
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
