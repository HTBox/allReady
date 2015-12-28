using Microsoft.AspNet.Mvc;
using AllReady.Areas.Admin.Models;
using MediatR;
using AllReady.Models;
using AllReady.Areas.Admin.Features.Organizations;
using Microsoft.AspNet.Authorization;

namespace AllReady.Areas.Admin.Controllers
{
    [Route("admin/api/organization")]
    [Produces("application/json")]
    [Area("Admin")]
    [Authorize("OrgAdmin")]

    public class OrganizationApiController : Controller
    {
        private readonly IMediator _bus;

        public OrganizationApiController(IMediator bus)
        {
            _bus = bus;
        }

        // GET api/values/5/Contact
        [HttpGet("{id}/Contact")]
        [Produces("application/json", Type = typeof(ContactInformationModel))]
        public ContactInformationModel GetContact(int id)
        {
            var contact = _bus.Send(new OrganizationContactQuery  { Id = id, ContactType = ContactTypes.Primary });
            return contact;
        }
    }
}
