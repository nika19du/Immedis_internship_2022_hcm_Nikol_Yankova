using App.Controllers.Validation;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public EmployeesController(IEmployeeService employeeService, Context context, ICloudinaryService cloudinaryService, IMapper mapper, Validate validate)
        {
            this.employeeService = employeeService;
            this.context = context;
            this.cloudinaryService = cloudinaryService;
            this.mapper = mapper;
            this.validate = validate;
        }
        private IEmployeeService employeeService;
        private readonly Context context;
        private ICloudinaryService cloudinaryService;
        private readonly IMapper mapper;
        private readonly Validate validate;
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            if (AccountService.Role == "Admin" && AccountService.UsrId!=null)
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
            var message = validate.CheckCreating(model);
            if (message != null)
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name", model.DepartmentId);
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name", model.DepartmentId);
                ModelState.AddModelError("Error",message);
                return this.View(model);
            }
            string pictureUrl = await this.cloudinaryService.UploadPictureAsync(model.Image, model.Username);
            await this.employeeService.CreateAsync(model.FirstName, model.LastName, model.Phone, model.Email, model.GenderType, model.StartDate, pictureUrl, model.Salary, model.Username, model.Password, model.DepartmentId, model.RoleId);
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
            if (AccountService.Role == "Admin" || AccountService.UsrId == id)
            {
                var employeesCreateInputModel = this.mapper.Map<Employee, EmployeesEditInputModel>(employee);
                employeesCreateInputModel.Department = context.Departments.Find(employee.DepartmentId);
                employeesCreateInputModel.Image = null;
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name");
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name");
                return View(employeesCreateInputModel);
            }
            else
            {
                ViewBag.ErrorTitle = $"Only admin and {employee.Username} can edit employee profile!";
                ViewBag.ErrorMessage = $"{AccountService.Username} can't access this page!";
                return View("NotFoundPage");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeesEditInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                ViewData["Department"] = new SelectList(context.Departments, "Id", "Name", model.DepartmentId);
                ViewData["Role"] = new SelectList(context.Roles, "Id", "Name", model.DepartmentId);
                return this.View(model);
            } ;
            string picUrl = null;
            if (model.Image != null)
            {
                picUrl = await this.cloudinaryService.UploadPictureAsync(model.Image, model.Username);
            }

            var employeeServiceModel = mapper.Map<EmployeesEditInputModel, EmployeeServiceModel>(model);
            employeeServiceModel.GenderType = model.GenderType;
            if (picUrl != null)
                employeeServiceModel.Image = picUrl;
            employeeService.EditAsync(employeeServiceModel, model.Id);
            return this.RedirectToAction("All", "Employees");
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
