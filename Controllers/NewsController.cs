using HaberPortali2.Models;
using HaberPortali2.Repositories;
using HaberPortali2.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HaberPortali2.Controllers
{
    public class NewsController : Controller
    {
        private readonly IRepository<News> _newsRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IHubContext<CommentHub> _hub;

        public NewsController(
            IRepository<News> newsRepo,
            IRepository<Comment> commentRepo,
            IHubContext<CommentHub> hub)
        {
            _newsRepo = newsRepo;
            _commentRepo = commentRepo;
            _hub = hub;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _newsRepo.GetAllAsync();
            return View(list.OrderByDescending(x => x.Id).ToList());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var news = await _newsRepo.GetByIdAsync(id);
            if (news == null)
                return NotFound();

            var comments = await _commentRepo.GetAllAsync();

            ViewBag.Comments = comments
                .Where(c => c.NewsId == id)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            return View(news);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] Comment model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
                return BadRequest();

            var comment = new Comment
            {
                NewsId = model.NewsId,
                Text = model.Text.Trim(),
                UserName = User.Identity!.Name!,
                CreatedDate = DateTime.Now
            };

            await _commentRepo.AddAsync(comment);
            await _commentRepo.SaveChangesAsync();

            bool isOwner = true;

            await _hub.Clients.All.SendAsync(
                "ReceiveComment",
                comment.Id,
                comment.UserName,
                comment.Text,
                comment.NewsId,
                comment.CreatedDate.ToString("dd.MM.yyyy HH:mm"),
                isOwner
            );

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditComment([FromBody] Comment model)
        {
            var comment = await _commentRepo.GetByIdAsync(model.Id);
            if (comment == null)
                return NotFound();

            if (comment.UserName != User.Identity!.Name)
                return Forbid();

            comment.Text = model.Text.Trim();
            comment.UpdatedDate = DateTime.Now;

            await _commentRepo.UpdateAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return Json(new
            {
                id = comment.Id,
                text = comment.Text
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteComment([FromBody] int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
                return NotFound();

            bool isAdmin = User.IsInRole("Admin");
            bool isOwner = comment.UserName == User.Identity!.Name;

            if (!isAdmin && !isOwner)
                return Forbid();

            await _commentRepo.DeleteAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return Json(new { success = true, id });
        }
    }
}
