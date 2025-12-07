using HaberPortali2.Data;
using HaberPortali2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NewsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ============================================================
        // LIST
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var newsList = await _context.News
                .Include(n => n.Category)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return View(newsList);
        }

        // ============================================================
        // ADD (GET)
        // ============================================================
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = _context.Categories
                .OrderBy(x => x.Name)
                .ToList();

            return View();
        }

        // ============================================================
        // ADD (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Add(News model, IFormFile? imageFile)
        {
            model.CreatedDate = DateTime.Now;
            model.CreatedByUserId = 1;

            // ❗ TASLAK SİSTEMİNİ KALDIRDIK → TÜM HABERLER YAYINDA
            model.IsPublished = true;

            // Resim yükleme
            if (imageFile != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                    await imageFile.CopyToAsync(stream);

                model.ImageUrl = "/uploads/" + fileName;
            }

            _context.News.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ============================================================
        // EDIT (GET)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();

            ViewBag.Categories = _context.Categories
                .OrderBy(x => x.Name)
                .ToList();

            return View(news);
        }

        // ============================================================
        // EDIT (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Edit(News model, IFormFile? imageFile)
        {
            var existing = await _context.News.FindAsync(model.Id);
            if (existing == null) return NotFound();

            // Yeni görsel yüklendiyse
            if (imageFile != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                    await imageFile.CopyToAsync(stream);

                // Eski resmi sil
                if (!string.IsNullOrEmpty(existing.ImageUrl))
                {
                    string oldPath = Path.Combine(_env.WebRootPath, existing.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                existing.ImageUrl = "/uploads/" + fileName;
            }

            // Alanları güncelle
            existing.Title = model.Title;
            existing.Content = model.Content;
            existing.CategoryId = model.CategoryId;

            // ❗ Düzenleme sonrası da haber yayında kalsın
            existing.IsPublished = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ============================================================
        // DELETE
        // ============================================================
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return NotFound();

            // Eski görseli sil
            if (!string.IsNullOrEmpty(news.ImageUrl))
            {
                string fullPath = Path.Combine(_env.WebRootPath, news.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
