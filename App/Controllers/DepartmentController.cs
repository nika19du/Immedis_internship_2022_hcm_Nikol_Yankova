using App.Helper;
using Data.Models;
using HCMA.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Controllers
{
    public class DepartmentController : Controller
    {
        public DepartmentController(AppAPI appAPI)
        {
            this.api = appAPI;
            this.client = api.Initial();
        }
        private AppAPI api;

        private HttpClient client;

        public async Task<IActionResult> Index()
        {
            List<Department> departments = new List<Department>();
            HttpResponseMessage res = await client.GetAsync("api/departments");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                departments = JsonConvert.DeserializeObject<List<Department>>(result);
            }
            return View(departments);
        }
        public IActionResult Create()
        {
            if (AccountService.Role == "Admin" || AccountService.Role == "Supervisor")
            {
                return View(); 
            }
            else
            {
                ViewBag.ErrorTitle = "Only admin and supervisor can add department in system! ";
                ViewBag.ErrorMessage = $"{AccountService.Username} can't access this page!";
                return View("NotFoundPage");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.Name))
            {
                ModelState.AddModelError(nameof(department.Name), "Fill the field!");
                return View(department);
            }
            var res = await client.PostAsJsonAsync("/api/departments", department);

            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(department);
        }

        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage res = await client.GetAsync($"api/departments/{id}");
            var result = res.Content.ReadAsStringAsync().Result;
            var d = JsonConvert.DeserializeObject<Department>(result);
            return this.View(d);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.Name))
            {
                ModelState.AddModelError(nameof(department.Name), "Fill the field!");
                return View(department);
            }
            var res = await client.PutAsJsonAsync($"/api/departments/{department.Id}", department);

            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(department);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var res = await client.DeleteAsync($"/api/departments/{id}");
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View();
        }
    }
}
