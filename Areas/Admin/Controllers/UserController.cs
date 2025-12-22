using HaberPortali2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            ViewBag.AdminIds = new HashSet<int>(
                (await _userManager.GetUsersInRoleAsync("Admin")).Select(x => x.Id)
            );

            ViewBag.AuthorIds = new HashSet<int>(
                (await _userManager.GetUsersInRoleAsync("Author")).Select(x => x.Id)
            );

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRole(int id, string role)
        {
            if (role != "Admin" && role != "Author")
                return RedirectToAction("Index");

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return RedirectToAction("Index");

            var currentUserId = int.Parse(_userManager.GetUserId(User));

            if (role == "Admin" && user.Id == currentUserId)
                return RedirectToAction("Index");

            if (!await _roleManager.RoleExistsAsync(role))
                return RedirectToAction("Index");

            var isInRole = await _userManager.IsInRoleAsync(user, role);

            if (isInRole)
                await _userManager.RemoveFromRoleAsync(user, role);
            else
                await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Index");
        }

    }
}
