using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Controllers
{
    public class DashboardHubController : Controller
    {
        [HttpGet]
        public IActionResult DashboardHub()
        {
            return View("Dashboards");
        }
    }
}