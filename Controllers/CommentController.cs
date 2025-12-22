using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IRepository<Comment> _commentRepo;

        public CommentController(IRepository<Comment> commentRepo)
        {
            _commentRepo = commentRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int newsId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return RedirectToAction("Detail", "News", new { id = newsId });

            var comment = new Comment
            {
                NewsId = newsId,
                Text = text,
                UserName = User.Identity!.Name!,
                CreatedDate = DateTime.Now
            };

            await _commentRepo.AddAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return RedirectToAction("Detail", "News", new { id = newsId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int newsId)
        {
            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
                return RedirectToAction("Detail", "News", new { id = newsId });

            var isAdmin = User.HasClaim("IsAdmin", "True");
            var isOwner = comment.UserName == User.Identity!.Name;

            if (!isAdmin && !isOwner)
                return Forbid();

            await _commentRepo.DeleteAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return RedirectToAction("Detail", "News", new { id = newsId });
        }
    }
}
