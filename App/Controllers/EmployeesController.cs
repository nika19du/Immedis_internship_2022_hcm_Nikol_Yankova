using Data;
using Data.Models;
using HCMA.InputModels.Employees;
using HCMA.Services;
using HCMA.Services.Cloudinaries;
using HCMA.Services.Employees;
using HCMA.Services.Employees.Model;
using HCMA.ViewModels.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
namespace App.Controllers
{
    public class EmployeesController : Controller
    {
        public EmployeesController(IEmployeeService employeeService, Context context, ICloudinaryService cloudinaryService)
        {
            this.employeeService = employeeService;
            this.context = context;
            this.cloudinaryService = cloudinaryService;
        }
        private IEmployeeService employeeService;
        private readonly Context context;
        private ICloudinaryService cloudinaryService;
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            if (AccountService.Username == "Admin")
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name");
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name");
                return View();
            }
            else
            {
                ViewBag.ErrorTitle = "Only admin can add employee in system!";
                ViewBag.ErrorMessage = $"{AccountService.Username} can't access this page!";
                return View("NotFoundPage");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeesCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name", model.DepartmentId);
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name", model.DepartmentId);
                return this.View(model);
            }
            string pictureUrl = await this.cloudinaryService.UploadPictureAsync(model.Image, model.Username);
            await this.employeeService.CreateAsync(model.FirstName, model.LastName, model.Phone, model.Email, model.StartDate, pictureUrl, model.Salary, model.Username, model.Password, model.DepartmentId, model.RoleId);
            return RedirectToAction(nameof(All));
        }
        [HttpGet]
        public async Task<IActionResult> All(int? i, string sortOrder, string? id = null)
        {

            IEnumerable<EmployeesInfoViewModel> employees = await this.employeeService.GetAllAsync<EmployeesInfoViewModel>(id);
            var viewModel = new EmployeesAllViewModel
            {
                Employees = employees
            };
            IPagedList<EmployeesInfoViewModel> emps = viewModel.Employees.ToList().ToPagedList(i ?? 1, 6);
            viewModel.Employees = emps;
            ViewData["NameOrder"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "name_desc":
                    viewModel.Employees.Select(x => x.Username.OrderByDescending(x => x));// viewModel.Employees.OrderByDescending(x => x.Username);
                    break;
                default:
                    viewModel.Employees.Select(x => x.Username.OrderBy(x => x));// viewModel.Employees.OrderByDescending(x => x.Username);
                    // viewModel.Employees.OrderBy(x => x.Username);
                    break;
            }
            return View(viewModel);
        }
        public IActionResult Logout()
        {
            employeeService.Logout();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            try
            {
                var result = employeeService.Login(username, password);

                if (result == -1)
                {
                    ModelState.AddModelError(nameof(username), "Employee not found");
                    return this.View();
                }
                else return RedirectToAction("Index", "Home");
            }
            catch
            { 
                return View("NotFoundPage");
            }
        }
        public IActionResult Edit(int id)
        {
            var employee = context.Employees.FirstOrDefault(x => x.Id == id);
            EmployeesEditInputModel employeesCreateInputModel = new EmployeesEditInputModel
            {
                Username = employee.Username,
                Email = employee.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Phone = employee.Phone,
                Salary = employee.Salary
            };
            ViewData["Department"] = new SelectList(context.Departments, "Id", "Name");
            ViewData["Role"] = new SelectList(context.Roles, "Id", "Name");
            return View(employeesCreateInputModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeesEditInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name", model.DepartmentId);
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name", model.DepartmentId);
                return this.View(model);
            }

            var employee = await context.Employees.FindAsync(model.Id);

            if (model.Image != null && employee != null)
            {
                string picUrl = await this.cloudinaryService.UploadPictureAsync(model.Image, model.Username);

                EmployeeServiceModel employeeServiceModel = new EmployeeServiceModel
                {
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Image = picUrl,
                    DepartmentId = model.DepartmentId,
                    RoleId = model.RoleId,
                    Salary = model.Salary,
                    Password = model.Password,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address
                };
                employeeService.EditAsync(employeeServiceModel, model.Id);
                return this.RedirectToAction("All", "Employees");
            }
            else if (model.Image == null && employee != null)
            {
                EmployeeServiceModel employeeServiceModel = new EmployeeServiceModel
                {
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DepartmentId = model.DepartmentId,
                    RoleId = model.RoleId,
                    Salary = model.Salary,
                    Password = model.Password,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address
                };
                employeeService.EditAsync(employeeServiceModel, employee.Id);
                return this.RedirectToAction("All", "Employees", new { id = employee.Id });
            }
            else return BadRequest();
        }

        public async Task<IActionResult> Delete(int id)
        {
            await employeeService.DeleteAsync(id);
            return this.RedirectToAction("All", "Employees");
        }

        public async Task<IActionResult> Details(int id)
        {
            var e = await this.employeeService.GetByIdAsync<Employee>(id);
            if (e == null)
            {
                return this.NotFound();
            }
            var viewModel = new EmployeeDetailsViewModel
            {
                Employee = e
            };
            return View(viewModel);
        }
    }
}
