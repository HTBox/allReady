using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statusCode?}")]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == (int) HttpStatusCode.NotFound)
            {
                return View("404");
            }

            return View("~/Views/Shared/Error.cshtml");
        }

#if DEBUG
        // Test actions
        [Route("/Error/Generate/{statusCode}")]
        public IActionResult Generate(int? statusCode)
        {
            return StatusCode(statusCode ?? 500);
        }

        [Route("/Error/Generate/Exception")]
        public IActionResult Exception()
        {
            throw new Exception("This exception has been thrown purposefully to test error handling.");
        }
#endif
    }
}
