using App.Models;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace App.Controllers
{
    public class HomeController : Controller
    { 
        public HomeController(ILogger<HomeController> logger,Context ctx)
        {
            _logger = logger;
            this.context = ctx;
            
        } 
        private readonly ILogger<HomeController> _logger;
        private readonly Context context;
        public IActionResult Index()
        { 
            ViewData["EmployeesCount"] = context.Employees.Count();
            ViewData["DepartmentsCount"] = context.Departments.Count();
            ViewData["RolesCount"] = context.Roles.Count();
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
}
