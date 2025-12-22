using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<News> _newsRepo;
        private readonly IRepository<Comment> _commentRepo;

        public UserController(
            UserManager<AppUser> userManager,
            IRepository<News> newsRepo,
            IRepository<Comment> commentRepo)
        {
            _userManager = userManager;
            _newsRepo = newsRepo;
            _commentRepo = commentRepo;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var newsList = await _newsRepo.GetAllAsync();
            var commentList = await _commentRepo.GetAllAsync();

            ViewBag.MyNews = newsList
                .Where(n => n.CreatedBy == user.UserName)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();

            ViewBag.MyComments = commentList
                .Where(c => c.UserName == user.UserName)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            return View(user);
        }

        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
                return RedirectToAction("Profile");

            bool isOwner = comment.UserName == User.Identity!.Name;

            if (!isOwner)
                return Forbid();

            return View(comment);
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(Comment model)
        {
            var comment = await _commentRepo.GetByIdAsync(model.Id);
            if (comment == null)
                return RedirectToAction("Profile");

            bool isOwner = comment.UserName == User.Identity!.Name;

            if (!isOwner)
                return Forbid();

            comment.Text = model.Text;
            comment.UpdatedDate = DateTime.Now;

            await _commentRepo.UpdateAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
                return RedirectToAction("Profile");

            bool isOwner = comment.UserName == User.Identity!.Name;
            bool isAdmin = User.IsInRole("Admin");

            if (!isOwner && !isAdmin)
                return Forbid();

            await _commentRepo.DeleteAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return RedirectToAction("Profile");
        }
    }
}
