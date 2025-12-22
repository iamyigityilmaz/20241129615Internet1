using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IRepository<News> _newsRepo;
        private readonly IRepository<Comment> _commentRepo;

        public ProfileController(
            IRepository<News> newsRepo,
            IRepository<Comment> commentRepo)
        {
            _newsRepo = newsRepo;
            _commentRepo = commentRepo;
        }

        public async Task<IActionResult> Index()
        {
            var userName = User.Identity!.Name!;

            var allNews = await _newsRepo.GetAllAsync();
            var allComments = await _commentRepo.GetAllAsync();

            ViewBag.MyNews = allNews
                .Where(x => x.CreatedBy == userName)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            ViewBag.MyComments = allComments
                .Where(x => x.UserName == userName)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return View();
        }
    }
}
