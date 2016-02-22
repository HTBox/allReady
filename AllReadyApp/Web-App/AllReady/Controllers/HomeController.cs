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
            return View(_dataAccess.Campaigns.Where(c => c.EndDateTime.UtcDateTime.Date > DateTime.UtcNow.Date && !c.Locked).ToViewModel().OrderBy(vm => vm.EndDate).ToList());
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Aesop()
        {
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
