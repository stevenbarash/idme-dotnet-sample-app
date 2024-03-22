using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using idme_dotnet_sample_app.Models;

namespace idme_dotnet_sample_app.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {


        if (HttpContext.User.Identity.IsAuthenticated)
        {
            Console.WriteLine("User is authenticated");

            var claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            ViewBag.Claims = claims;
        }
        else
        {
            Console.WriteLine("User is not authenticated");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
