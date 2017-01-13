using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Events;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Features.Sms;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    [Route("Admin/Requests")]
    public class RequestController : Controller
    {
        public static string CreateRequestTitle = "Create New Request";
        public const string EditRequestPostRouteName = "EditRequestPost";

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
                EventName = campaignEvent.Name
            };

            ViewBag.Title = CreateRequestTitle;

            return View("Edit", model);
        }

        /// <summary>
        ///  Returns the view to allow a request to be modified.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _mediator.SendAsync(new EditRequestQuery { Id = id });
            if (model == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(model.OrganizationId))
            {
                return Unauthorized();
            }

            ViewData["Title"] = "Edit " + model.Name;

            return View("Edit", model);
        }

        /// <summary>
        /// Handles edit request POST
        /// </summary>
        /// <param name="model">The request edit model</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Edit", Name = EditRequestPostRouteName)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            var @event = await _mediator.SendAsync(new EventSummaryQuery { EventId = model.EventId });
            if (@event == null)
            {
                return BadRequest("Invalid event id");
            }

            if (!User.IsOrganizationAdmin(@event.OrganizationId))
            {
                return Unauthorized();
            }

            var validatePhoneNumberResult = await _mediator.SendAsync(new ValidatePhoneNumberRequest { PhoneNumber = model.Phone, ValidateType = true });
            if (!validatePhoneNumberResult.IsValid)
            {
                var phoneAttribute = model.GetType().GetProperties().Single(p => p.Name == nameof(model.Phone)).GetCustomAttributes(false).Cast<Attribute>().Single(a => a is PhoneAttribute) as PhoneAttribute;
                ModelState.AddModelError(nameof(model.Phone), phoneAttribute.ErrorMessage);
                return View("Edit", model);
            }

            await _mediator.SendAsync(new EditRequestCommand { RequestModel = model });

            return RedirectToAction("Requests", "Event", new { id = model.EventId });
        }
    }
}