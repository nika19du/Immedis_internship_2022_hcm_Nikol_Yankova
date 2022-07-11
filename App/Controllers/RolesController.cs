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
    public class RolesController : Controller
    {
        public RolesController(AppAPI appAPI)
        {
            this.api = appAPI;
            this.client = api.Initial();
        }
        private AppAPI api;

        private HttpClient client;
       
        public async Task<IActionResult> Index()
        {
            List<Role> roles = new List<Role>(); 
            HttpResponseMessage res = await client.GetAsync("api/roles");
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                roles = JsonConvert.DeserializeObject<List<Role>>(result);
            }
            return View(roles);
        }
        public IActionResult Create()
        {
            if(AccountService.Role!="Admin")
            {
                ViewBag.ErrorTitle = "Only admin can add role in system! ";
                ViewBag.ErrorMessage = $"{AccountService.Username} can't access this page!";
                return View("NotFoundPage");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        { 
            if (string.IsNullOrWhiteSpace(role.Name))
            {
                ModelState.AddModelError(nameof(role.Name), "Fill the field!");
                return View(role);
            }
            var res = await client.PostAsJsonAsync("/api/roles", role);

            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(role);
        } 

        public async Task<IActionResult> Edit(int id)
        {
            if (AccountService.Role != "Admin")
            {
                ViewBag.ErrorTitle = "Only admin can add employee in system! ";
                ViewBag.ErrorMessage = $"{AccountService.Username} can't access this page!";
                return View("NotFoundPage");
            }
            HttpResponseMessage res = await client.GetAsync($"api/roles/{id}");
            var result = res.Content.ReadAsStringAsync().Result;
            var d = JsonConvert.DeserializeObject<Role>(result);
            return this.View(d);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Role role)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
            {
                ModelState.AddModelError(nameof(role.Name), "Fill the field!");
                return View(role);
            } 
            var res = await client.PutAsJsonAsync($"/api/roles/{role.Id}",role);

            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(role);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (AccountService.Role != "Admin")
            {
                ViewBag.ErrorTitle = "Only admin can add employee in system! ";
                ViewBag.ErrorMessage = $"{AccountService.Username} can't access this page!";
                return View("NotFoundPage");
            }
            var res = await client.DeleteAsync($"/api/roles/{id}");
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View();
        }
    }
}
