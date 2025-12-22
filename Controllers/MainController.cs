using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Controllers
{
    public class MainController : Controller
    {
        private readonly IRepository<News> _newsRepo;

        public MainController(IRepository<News> newsRepo)
        {
            _newsRepo = newsRepo;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _newsRepo.GetAllAsync();

            var last10 = news
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToList();

            return View(last10);
        }

        public async Task<IActionResult> Category(int id)
        {
            var news = await _newsRepo.GetAllAsync();

            var filtered = news
                .Where(x => x.CategoryId == id)
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(filtered);
        }
    }
}
