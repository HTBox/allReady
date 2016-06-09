using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    /// <summary>
    /// The token controller for the API is to allow third-party services
    /// with the proper claim to generate transient tokens on-demand to 
    /// access other services within AllReady.
    /// </summary>

    [Route("api/token")]
    public class TokenApiController : Controller
    {

    }
}
