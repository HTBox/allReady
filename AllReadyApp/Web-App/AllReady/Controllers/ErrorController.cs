using Microsoft.AspNetCore.Mvc;

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