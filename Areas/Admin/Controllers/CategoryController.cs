using HaberPortali2.Models;
using HaberPortali2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _repo;

        public CategoryController(IRepository<Category> repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAllAsync();

            // En yeni kategori en üstte olsun
            var orderedList = list
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(orderedList);
        }

        public IActionResult Add() => View();

        [HttpPost]
        public async Task<IActionResult> Add(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _repo.GetByIdAsync(id);

            if (item == null)
                return RedirectToAction("Index");

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.UpdateAsync(model);
            await _repo.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _repo.GetByIdAsync(id);

            if (item != null)
            {
                await _repo.DeleteAsync(item);
                await _repo.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
