using System;
using System.Collections.Generic;
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

                Console.WriteLine("User is INDEED authenticated");

                // ReSharper disable once InvalidXmlDocComment
                /// <summary>
                /// Retrieves the claims from the current user and strips the URL to make the claim more readable.
                /// </summary>
                /// <returns>A list of claims with the URL stripped.</returns>
                var claims = HttpContext.User.Claims.Select(c => new
                {
                    Type = c.Type.Replace("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/", string.Empty),
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