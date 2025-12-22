using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IRepository<News> _newsRepo;

        public HomeController(IRepository<News> newsRepo)
        {
            _newsRepo = newsRepo;
        }

        public async Task<IActionResult> Index()
        {
            var last = await _newsRepo.GetAllAsync();
            var lastFive = last
                .OrderByDescending(x => x.Id)
                .Take(5);

            return View(lastFive);
        }
    }
}
