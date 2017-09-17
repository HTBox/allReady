using AllReady.Areas.Admin.Features.Invite;
using AllReady.Areas.Admin.ViewModels.Invite;
using AllReady.Features.Campaigns;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AllReady.Constants;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize(nameof(UserType.OrgAdmin))]
    public class InviteController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public InviteController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        // GET: Admin/Invite/SendCampaignManagerInvite/5
        [Route("[area]/[controller]/[action]/{campaignId:int}")]
        public async Task<IActionResult> SendCampaignManagerInvite(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignByCampaignIdQuery { CampaignId = campaignId });
            if (campaign == null) return NotFound();

            if (!IsUserAuthorizedToSendInvite(campaign.ManagingOrganizationId))
            {
                return Unauthorized();
            }

            return View("Send", new InviteViewModel
            {
                CampaignId = campaignId,
                Title = "Send Campaign Manager Invite",
                FormAction = "SendCampaignManagerInvite",
            });
        }

        // GET: Admin/Invite/SendEventManagerInvite/5
        [Route("[area]/[controller]/[action]/{eventId:int}")]
        public async Task<IActionResult> SendEventManagerInvite(int eventId)
        {
            var @event = await _mediator.SendAsync(new EventByEventIdQuery { EventId = eventId });
            if (@event == null) return NotFound();

            if (!IsUserAuthorizedToSendInvite(@event.Campaign.ManagingOrganizationId))
            {
                return Unauthorized();
            }

            return View("Send", new InviteViewModel
            {
                EventId = eventId,
                Title = "Send Event Manager Invite",
                FormAction = "SendEventManagerInvite",
            });
        }

        // POST: Admin/Invite/SendCampaignManagerInvite/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[area]/[controller]/[action]/{campaignId:int}")]
        public async Task<IActionResult> SendCampaignManagerInvite(int campaignId, InviteViewModel invite)
        {
            if (invite == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var campaign = await _mediator.SendAsync(new CampaignByCampaignIdQuery { CampaignId = campaignId });

                if (campaign == null)
                {
                    return BadRequest();
                }

                if (!IsUserAuthorizedToSendInvite(campaign.ManagingOrganizationId))
                {
                    return Unauthorized();
                }

                invite.CampaignId = campaignId;
                var user = await _userManager.GetUserAsync(User);
                await _mediator.SendAsync(new CreateCampaignManagerInviteCommand { Invite = invite, UserId = user?.Id });

                return RedirectToAction(actionName: "Details", controllerName: "Campaign", routeValues: new { area = AreaNames.Admin, id = invite.CampaignId });
            }

            return View("Send", invite);
        }

        // POST: Admin/Invite/SendEventManagerInvite/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[area]/[controller]/[action]/{eventId:int}")]
        public async Task<IActionResult> SendEventManagerInvite(int eventId, InviteViewModel invite)
        {
            if (invite == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var @event = await _mediator.SendAsync(new EventByEventIdQuery { EventId = eventId });
                if (@event == null)
                {
                    return BadRequest();
                }

                if (!IsUserAuthorizedToSendInvite(@event.Campaign.ManagingOrganizationId))
                {
                    return Unauthorized();
                }

                invite.EventId = eventId;
                var user = await _userManager.GetUserAsync(User);
                await _mediator.SendAsync(new CreateEventManagerInviteCommand { Invite = invite, UserId = user?.Id });

                return RedirectToAction(actionName: "Details", controllerName: "Event", routeValues: new { area = AreaNames.Admin, id = invite.EventId });
            }

            return View("Send", invite);
        }

        private bool IsUserAuthorizedToSendInvite(int organizationId)
        {
            var userOrganizationId = User.GetOrganizationId();

            if (!User.IsOrganizationAdmin(organizationId))
            {
                return false;
            }

            return true;
        }

    }
}