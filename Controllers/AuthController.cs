using HaberPortali2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HaberPortali2.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View();
            }

            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Main");
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string userName, string password)
        {
            var user = new AppUser
            {
                FullName = fullName,
                Email = email,
                UserName = userName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join("<br>", result.Errors.Select(x => x.Description));
                return View();
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index", "Main");
        }

        public IActionResult GoogleLogin()
        {
            var props = _signInManager.ConfigureExternalAuthenticationProperties(
                "Google",
                Url.Action(nameof(ExternalLoginCallback))
            );

            return Challenge(props, "Google");
        }

        public IActionResult FacebookLogin()
        {
            var props = _signInManager.ConfigureExternalAuthenticationProperties(
                "Facebook",
                Url.Action(nameof(ExternalLoginCallback))
            );

            return Challenge(props, "Facebook");
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                true
            );

            if (result.Succeeded)
                return RedirectToAction("Index", "Main");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email
                };

                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, "User");
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index", "Main");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Main");
        }
    }
}
