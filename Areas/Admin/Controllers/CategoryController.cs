using HaberPortali2.Data;
using HaberPortali2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]                     // *** Çok önemli ***
    [Authorize]                         // Admin girişi olmadan erişilemez
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // KATEGORİ LİSTELEME
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(categories);
        }

        // ============================================================
        // KATEGORİ EKLEME (GET)
        // ============================================================
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // ============================================================
        // KATEGORİ EKLEME (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Add(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Aktif kategorilerde isim kontrolü
            var exists = await _context.Categories
                .FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == model.Name.ToLower()
                    && x.IsActive == true);

            if (exists != null && model.IsActive)
            {
                ViewBag.Error = "Bu isimde zaten aktif bir kategori mevcut!";
                return View(model);
            }

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Category", new { area = "Admin" });
        }

        // ============================================================
        // KATEGORİ DÜZENLEME (GET)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // ============================================================
        // KATEGORİ DÜZENLEME (POST)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.Categories.FindAsync(model.Id);

            if (existing == null)
                return NotFound();

            // Aynı isimde başka aktif kategori var mı?
            var exists = await _context.Categories
                .FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == model.Name.ToLower()
                    && x.IsActive == true
                    && x.Id != model.Id);

            if (exists != null && model.IsActive)
            {
                ViewBag.Error = "Bu isimde zaten aktif bir kategori mevcut!";
                return View(model);
            }

            existing.Name = model.Name;
            existing.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Category", new { area = "Admin" });
        }

        // ============================================================
        // KATEGORİ SİLME
        // ============================================================
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Category", new { area = "Admin" });
        }
    }
}
