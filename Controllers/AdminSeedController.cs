using HaberPortali2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Controllers
{
    public class AdminSeedController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminSeedController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> MakeAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Content("Kullanıcı yok");

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            return Content("Admin yapıldı");
        }
    }
}
