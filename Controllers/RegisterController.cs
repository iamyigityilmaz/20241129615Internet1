using HaberPortali2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HaberPortali2.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(string fullName, string email, string password)
        {
            var user = new AppUser
            {
                FullName = fullName,
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                return RedirectToAction("Login", "Auth");

            return View();
        }
    }
}
