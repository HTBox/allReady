using AllReady.Features.Campaigns;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AllReady.Features.Home;
using AllReady.ViewModels.Home;

namespace AllReady.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error404()
        {
            return View("404");
        }
    }
}