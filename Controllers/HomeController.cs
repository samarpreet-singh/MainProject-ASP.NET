﻿/*
Student Name: Samarpreet Singh
Student Number: 200510621
Program: CMPG 2 Year Diploma
Section and CRN: Wednesday 7PM, 11437
Assignment 1
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
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
