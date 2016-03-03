using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Route("admin/api/organization")]
    [Produces("application/json")]
    [Area("Admin")]
    [Authorize("OrgAdmin")]

    public class OrganizationApiController : Controller
    {
        private readonly IMediator _mediator;

        public OrganizationApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/values/5/Contact
        [HttpGet("{id}/Contact")]
        [Produces("application/json", Type = typeof(ContactInformationModel))]
        public ContactInformationModel GetContact(int id)
        {
            var contact = _mediator.Send(new OrganizationContactQuery  { Id = id, ContactType = ContactTypes.Primary });
            return contact;
        }
    }
}
