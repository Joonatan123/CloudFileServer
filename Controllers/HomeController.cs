using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CloudFileServer.Models;

namespace CloudFileServer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private string password;

    public HomeController(ILogger<HomeController> logger, IConfiguration config)
    {
        _logger = logger;
        password = config.GetValue<string>("Password", "pass");
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("loggedIn") == null)
            return Redirect("/Home/LogIn");
        return Redirect("/Download");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult LogIn()
    {
        if (HttpContext.Session.GetInt32("loggedIn") != null)
            return Redirect("/Download");
        return View();
    }
    [HttpPost]
    public IActionResult LogIn(string password)
    {
        if (password != null && password.Equals(this.password))
        {
            HttpContext.Session.SetInt32("loggedIn", 73);
            return Redirect("/Download");
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
