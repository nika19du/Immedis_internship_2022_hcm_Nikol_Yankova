using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Controllers
{
    [Route("/ErrorPage/{statusCode}")]
    public class ErrorPageController : Controller
    {
        public IActionResult Index(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewData["Error"] = "Page Not Found";
                    break; 
                default:
                    break;
            }
            return View("NotFoundPage");
        }
    }
}
