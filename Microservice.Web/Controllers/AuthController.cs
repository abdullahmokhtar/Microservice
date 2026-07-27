using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microservice.Web.Controllers
{
    public class AuthController(IAuthService authService, ITokenProvider tokenProvider) : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await authService.LoginAsync(model);
            if (result == null || !result.Success)
            {
                TempData["error"] = result.Message;
                return View(model);
            }
            await SignInUser(result.Data?.Token ?? string.Empty);
            tokenProvider.SetToken(result.Data?.Token);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem {Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                new SelectListItem {Text = SD.RoleCustomer, Value = SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await authService.RegisterAsync(model);
            if (result == null || !result.Success)
            {
                var roleList = new List<SelectListItem>()
                {
                    new SelectListItem {Text = SD.RoleAdmin, Value = SD.RoleAdmin},
                    new SelectListItem {Text = SD.RoleCustomer, Value = SD.RoleCustomer}
                };
                ViewBag.RoleList = roleList;
                TempData["error"] = result.Message;
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Role))
            {
                model = model with { Role = SD.RoleCustomer };
            }
            await authService.AssignRoleAsync(model);
            TempData["Success"] = "Registration successful. Please log in.";

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject));
            identity.AddClaim(new Claim(ClaimTypes.Email, jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value));
            var principle = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
        }
    }
}
