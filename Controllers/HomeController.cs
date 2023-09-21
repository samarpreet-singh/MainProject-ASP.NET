/*
Student Name: Samarpreet Singh
Student Number: 200510621
Program: CMPG 2 Year Diploma
Section and CRN: Wednesday 7PM, 11437
Assignment 1 - BONUS is the privacy page (privacy.cshtml)
*/
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MainProject.Models;

namespace MainProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Adding a new controller action called "Brief"
    public IActionResult Brief()
    {
        List<Dictionary<string, object>> companyInfoData = new()
            {
                new() { { "Company Name", "Crafty Creations"}, { "Company Description", "Crafty Creations is an online marketplace dedicated to promoting and selling handmade crafts from artisans around the world. It provides a much-needed platform for artisans to showcase and sell their unique creations, from cosplay outfits and fictional character figures to home decor. All crafts/items will be certified to make sure they are of the highest quality!" }, { "Group Members and Roles", "Samarpreet Singh, Full-Stack Developer" } },
            };

        return View(companyInfoData);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
