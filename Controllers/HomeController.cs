using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using idme_dotnet_sample_app.Models;

namespace idme_dotnet_sample_app.Controllers;

/// <summary>
/// The HomeController class inherits from the Controller base class and represents a controller for the application home pages.
/// </summary>
/// <remarks>
/// The HomeController has methods to handle requests, actions, and processes that pertain to the application's home pages.
/// </remarks>

public class HomeController : Controller
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>

    /// <summary>
    /// Action method for the home page.
    /// </summary>
    /// <returns>The view result.</returns>
    public IActionResult Index()
    {
        if (HttpContext.User.Identity is { IsAuthenticated: true })
        {
            Console.WriteLine("User is authenticated");

            var claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            ViewBag.Claims = claims;
            ViewBag.Token = HttpContext.Request.Headers["Authorization"];
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
//    public IActionResult Privacy()
//    {
//        return View();
//    }

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
