using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using AllReady.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AllReady.Areas.Admin.Controllers
{
    public class UserAdminController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            return View(new List<ApplicationUser>());
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string UserId, string Role)
        {
            throw new NotImplementedException();
        }
    }
}
