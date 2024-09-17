using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace idme_dotnet_sample_app.Controllers
{

    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            if (HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                return Challenge("IDme");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            // Sign out of the cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Sign out of the ID.me OpenID Connect authentication
            return SignOut(
                new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") },
                "IDme"
            );
        }

        public IActionResult AccessDenied()
        {
            return Content("Access denied");
        }
    }
}