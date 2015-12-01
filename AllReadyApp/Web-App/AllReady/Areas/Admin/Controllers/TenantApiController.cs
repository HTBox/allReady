using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using AllReady.Areas.Admin.Models;
using MediatR;
using AllReady.Models;
using AllReady.Areas.Admin.Features.Tenants;
using Microsoft.AspNet.Authorization;

namespace AllReady.Areas.Admin.Controllers
{
    [Route("admin/api/tenant")]
    [Produces("application/json")]
    [Area("Admin")]
    [Authorize("TenantAdmin")]

    public class TenantApiController : Controller
    {

        private readonly IMediator _bus;

        public TenantApiController(IMediator bus)
        {
            _bus = bus;
        }

        // GET api/values/5/Contact
        [HttpGet("{id}/Contact")]
        [Produces("application/json", Type = typeof(ContactInformationModel))]
        public ContactInformationModel GetContact(int id)
        {
            var contact = _bus.Send(new TenantContactQuery { Id = id, ContactType = ContactTypes.Primary });
            return contact;
        }


    }
}
