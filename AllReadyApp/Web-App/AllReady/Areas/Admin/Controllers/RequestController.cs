using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AllReady.Areas.Admin.ViewModels.Request;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    [Route("Admin/Requests")]
    public class RequestController : Controller
    {
        public static string CreateRequestTitle = "Create New Request";

        private readonly IMediator _mediator;

        /// <summary>
        /// Constructs a new instance of the <see cref="RequestController"/>.
        /// </summary>
        /// <param name="mediator"></param>
        public RequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Returns the view to allow a new request to be added.
        /// </summary>
        /// <param name="eventId">The id of the event which the request will be linked to.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create(int eventId)
        {
            var campaignEvent = await _mediator.SendAsync(new EventSummaryQuery { EventId = eventId });

            if (campaignEvent == null)
            {
                return BadRequest("Invalid event id");
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return Unauthorized();
            }

            var model = new EditRequestViewModel
            {
                OrganizationId = campaignEvent.OrganizationId,
                OrganizationName = campaignEvent.OrganizationName,
                CampaignId = campaignEvent.CampaignId,
                CampaignName = campaignEvent.CampaignName,
                EventId = campaignEvent.Id,
                EventName = campaignEvent.Name,
            };

            ViewBag.Title = CreateRequestTitle;

            return View("Edit", model);
        }

        /// <summary>
        /// Handles edit request POST requests
        /// </summary>
        /// <param name="model">The request edit model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRequestViewModel model)
        {
            var campaignEvent = await _mediator.SendAsync(new EventSummaryQuery() { EventId = model.EventId });

            if (campaignEvent == null)
            {
                return BadRequest("Invalid event id");
            }

            if (!User.IsOrganizationAdmin(campaignEvent.OrganizationId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            await _mediator.SendAsync(new EditRequestCommand { RequestModel = model });

            return RedirectToAction("Details", "Event", new { id = model.EventId });
        }
    }
}
