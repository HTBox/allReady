using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace AllReady.Controllers
{
    [Authorize]
    public class MeApiController : Controller
    {
        [Route("api/me")]
        public string Index()
        {
            return Request.Cookies[".AspNet.ApplicationCookie"];
        }
    }
}
