using HCMA.Services.Employees;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Controllers
{
    public class EmployeesController : Controller
    {
        public EmployeesController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }
        private IEmployeeService employeeService;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        } 
    }
}
