using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

using System;

namespace AllReady.Controllers
{
    [Authorize]
    public class MeApiController : Controller
    {
        // GET: Me
        [Route("api/me")]
        public string Index()
        {
            return Request.Cookies.Get(".AspNet.ApplicationCookie");
        }
    }
}
