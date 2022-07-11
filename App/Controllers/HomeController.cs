using App.Models;
using Data;
using Data.Models;
using HCMA.Services;
using HCMA.Services.Employees;
using HCMA.ViewModels.Employees;
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
        public HomeController(ILogger<HomeController> logger,Context ctx,IEmployeeService employeeService)
        {
            _logger = logger;
            this.context = ctx; 
            this.employeeService = employeeService;
        } 
        private readonly ILogger<HomeController> _logger;
        private readonly Context context; 
        private IEmployeeService employeeService;
        public async Task<IActionResult> Index()
        { 
            ViewData["EmployeesCount"] = context.Employees.Count();
            ViewData["DepartmentsCount"] = context.Departments.Count();
            ViewData["RolesCount"] = context.Roles.Count();
            if(AccountService.UsrId!=null && AccountService.Role=="Staff")
            {
                int id = int.Parse(AccountService.UsrId.ToString());
                var e = await this.employeeService.GetByIdAsync<Employee>(id);
                var viewModel = new EmployeeDetailsViewModel
                {
                    Employee = e
                }; 
                return RedirectToAction("Details","Employees",new { id=id });
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
}
