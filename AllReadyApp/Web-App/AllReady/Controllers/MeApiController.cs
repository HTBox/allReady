using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Authorize]
    public class MeApiController : Controller
    {
        // GET: Me
        [Route("api/me")]
        public string Index()
        {
            return Request.Cookies[".AspNet.ApplicationCookie"];
        }
    }
}
