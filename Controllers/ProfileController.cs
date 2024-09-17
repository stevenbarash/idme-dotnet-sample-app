using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace idme_dotnet_sample_app.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                Console.WriteLine("User is authenticated");

                var claims = HttpContext.User.Claims.Select(c => new
                {
                    Type = c.Type.Split('/').Last().Split('\\').Last(),
                    c.Value
                }).ToList();
                ViewBag.Claims = claims;
            }
            else
            {
                Console.WriteLine("User is not authenticated");
            }

            return View();
        }
    }
}