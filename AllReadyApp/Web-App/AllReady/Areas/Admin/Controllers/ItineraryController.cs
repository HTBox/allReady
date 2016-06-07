using AllReady.Areas.Admin.Features.Itineraries;
using AllReady.Security;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class ItineraryController : Controller
    {
        private readonly IMediator _mediator;       

        public ItineraryController(IMediator mediator)
        {            
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            _mediator = mediator;            
        }

        [HttpGet]
        [Route("Admin/Itinerary/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var Itinerary = await _mediator.SendAsync(new ItineraryDetailQuery { ItineraryId = id });

            if (Itinerary == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(Itinerary.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View("Details", Itinerary);
        }
    }
}
