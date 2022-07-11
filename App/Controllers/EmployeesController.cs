using App.Models;
using AutoMapper;
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
        public EmployeesController(IEmployeeService employeeService, Context context, ICloudinaryService cloudinaryService,IMapper mapper)
        {
            this.employeeService = employeeService;
            this.context = context;
            this.cloudinaryService = cloudinaryService;
            this.mapper = mapper;
        }
        private IEmployeeService employeeService;
        private readonly Context context;
        private ICloudinaryService cloudinaryService;
        private readonly IMapper mapper;
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            if (AccountService.Role == "Admin")
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
            if (context.Employees.Any(x => x.Username == model.Username))
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name", model.DepartmentId);
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name", model.DepartmentId);
                ModelState.AddModelError(nameof(model.Username), "Someone already has that username. Try another?");
                return this.View(model);
            }
            if(model.Password.Length < 6 && model.Password.Any(char.IsUpper)==false && model.Password.Any(char.IsSymbol)==false)
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name", model.DepartmentId);
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name", model.DepartmentId);
                ModelState.AddModelError(nameof(model.Password), "Password must be more than 6 characters long, should contain at-least 1 Uppercase,1 Lowercase,1 Numeric and 1 special character.");
                return this.View(model);
            }
            if (model.Salary < 800)
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name", model.DepartmentId);
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name", model.DepartmentId);
                ModelState.AddModelError(nameof(model.Salary), "Salary can't be less than 800lv.");
                return this.View(model);
            }
            string pictureUrl = await this.cloudinaryService.UploadPictureAsync(model.Image, model.Username);
            await this.employeeService.CreateAsync(model.FirstName, model.LastName, model.Phone, model.Email,model.GenderType, model.StartDate, pictureUrl, model.Salary, model.Username, model.Password, model.DepartmentId, model.RoleId);
            return RedirectToAction(nameof(All));
        }
         
        [HttpGet]
        public async Task<IActionResult> All(int? i, string sortOrder, string? id = null)
        { 
            IEnumerable<EmployeesInfoViewModel> employees = await this.employeeService.GetAllAsync<EmployeesInfoViewModel>(id);
           
                var filters = new Filters(id);
                ViewBag.Filters = filters;
                var a = context.Departments.Select(arg => arg.Name).ToList();
                ViewData["Positions"] = new SelectList(a);
                var roles = context.Roles.Select(x => x.Name).ToList();
                ViewData["Roles"] = new SelectList(roles); 

       
            //IQueryable<Employee> query = context.Employees.Include(x => x.Department).Include(x => x.Role);
            //if (filters.HasPosition)
            //{
            //    query = query.Where(x => x.DepartmentId == int.Parse(filters.PositionId));
            //} 
            ViewData["NameOrder"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(x => x.Username).ToList();
                    break;
                default:
                    employees = employees.OrderBy(x => x.Username).ToList();
                    break;
            }
            var viewModel = new EmployeesAllViewModel
            {
                Employees = employees
            };
            IPagedList<EmployeesInfoViewModel> emps = viewModel.Employees.ToList().ToPagedList(i ?? 1, 6);
            viewModel.Employees = emps;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join('-', filter);
            return RedirectToAction("All", new { id = id });
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
                Salary = employee.Salary,
                Department = context.Departments.Find(employee.DepartmentId),
                DateOfBirth = employee.DateOfBirth,
                Address = employee.Address,
                StartDate = employee.StartDate
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
                    Address = model.Address,
                    GenderType=model.GenderType
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
                    Address = model.Address,
                    GenderType=model.GenderType
                };
                employeeService.EditAsync(employeeServiceModel, employee.Id);
                return this.RedirectToAction("All", "Employees");
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
