using HaberPortali2.Data;
using HaberPortali2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HaberPortali2.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------------------
        // 1) YORUM EKLEME (AJAX POST)
        // -----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AddComment(int newsId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Json(new { success = false, message = "Yorum boş olamaz." });
            }

            // Kullanıcı giriş yaptıysa email/username'i al
            string userName = User.Identity.IsAuthenticated
                ? User.Identity.Name
                : "Ziyaretçi";

            var comment = new Comment
            {
                NewsId = newsId,
                UserName = userName,
                Text = text,
                CreatedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                userName = comment.UserName,
                text = comment.Text,
                date = comment.CreatedDate.ToString("dd.MM.yyyy HH:mm")
            });
        }

        // -----------------------------------------------------
        // 2) HABERİN TÜM YORUMLARINI LİSTELEME
        // -----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetComments(int newsId)
        {
            var comments = await _context.Comments
                .Where(c => c.NewsId == newsId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return PartialView("_CommentListPartial", comments);
        }
    }
}
