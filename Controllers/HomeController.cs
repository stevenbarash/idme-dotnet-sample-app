using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using idme_dotnet_sample_app.Models;

namespace idme_dotnet_sample_app.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Action method for the home page.
    /// </summary>
    /// <returns>The view result.</returns>
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

    /// <summary>
    /// Action method for the privacy page.
    /// </summary>
    /// <returns>The view result.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Action method for handling errors.
    /// </summary>
    /// <returns>The view result with error details.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
