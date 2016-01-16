using System;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public HomeController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IActionResult Index()
        {
            return View(_dataAccess.Campaigns.Where(c => c.EndDateTime.UtcDateTime.Date > DateTime.UtcNow.Date).ToViewModel().OrderBy(vm => vm.EndDate).ToList());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Aesop()
        {
            ViewData["Message"] = "The backstory on why there are ants on the home page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult AccessDenied()
        {
            return View("~/Views/Shared/AccessDenied.cshtml");
        }
    }
}
