using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<News> _newsRepo;

        public CommentController(
            IRepository<Comment> commentRepo,
            IRepository<News> newsRepo)
        {
            _commentRepo = commentRepo;
            _newsRepo = newsRepo;
        }

        public async Task<IActionResult> Index()
        {
            var comments = (await _commentRepo.GetAllAsync())
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            var news = await _newsRepo.GetAllAsync();
            ViewBag.News = news.ToList();

            return View(comments);
        }

        public async Task<IActionResult> ByNews(int id)
        {
            var news = await _newsRepo.GetByIdAsync(id);
            if (news == null)
                return RedirectToAction("Index");

            ViewBag.NewsTitle = news.Title;

            var comments = (await _commentRepo.GetAllAsync())
                .Where(c => c.NewsId == id)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            return View(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment != null)
            {
                await _commentRepo.DeleteAsync(comment);
                await _commentRepo.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
