using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Author")]
    public class NewsController : Controller
    {
        private readonly IRepository<News> _newsRepo;
        private readonly IRepository<Category> _catRepo;

        public NewsController(
            IRepository<News> newsRepo,
            IRepository<Category> catRepo)
        {
            _newsRepo = newsRepo;
            _catRepo = catRepo;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _newsRepo.GetAllWithIncludeAsync(x => x.Category);

            if (User.IsInRole("Author") && !User.IsInRole("Admin"))
            {
                list = list
                    .Where(x => x.CreatedBy == User.Identity!.Name)
                    .ToList();
            }

            return View(list.OrderByDescending(x => x.Id).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = await _catRepo.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(News model, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/news");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                model.ImageUrl = "/uploads/news/" + fileName;
            }

            model.CreatedDate = DateTime.Now;
            model.CreatedBy = User.Identity!.Name!;

            await _newsRepo.AddAsync(model);
            await _newsRepo.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Categories = await _catRepo.GetAllAsync();
            var item = await _newsRepo.GetByIdAsync(id);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(News model, IFormFile imageFile)
        {
            var news = await _newsRepo.GetByIdAsync(model.Id);
            if (news == null)
                return RedirectToAction("Index");

            if (User.IsInRole("Author") && !User.IsInRole("Admin"))
            {
                if (news.CreatedBy != User.Identity!.Name)
                    return Forbid();
            }

            news.Title = model.Title;
            news.Content = model.Content;
            news.CategoryId = model.CategoryId;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/news");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                news.ImageUrl = "/uploads/news/" + fileName;
            }

            await _newsRepo.UpdateAsync(news);
            await _newsRepo.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _newsRepo.GetByIdAsync(id);
            if (item != null)
            {
                await _newsRepo.DeleteAsync(item);
                await _newsRepo.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
