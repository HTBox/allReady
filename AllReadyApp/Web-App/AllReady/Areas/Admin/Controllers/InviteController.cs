using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AllReady.Models;
using MediatR;
using AllReady.Areas.Admin.ViewModels.Invite;
using AllReady.Features.Events;
using AllReady.Features.Campaigns;
using Microsoft.AspNetCore.Identity;
using AllReady.Areas.Admin.Features.Invite;
using AllReady.Security;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                InviteType = InviteType.CampaignManagerInvite,
                CampaignId = campaignId,
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
                InviteType = InviteType.EventManagerInvite,
                EventId = eventId,
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
                var campaign = await _mediator.SendAsync(new CampaignByCampaignIdQuery { CampaignId = invite.CampaignId });

                if (campaign == null)
                {
                    return BadRequest();
                }

                if (!IsUserAuthorizedToSendInvite(campaign.ManagingOrganizationId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.GetUserAsync(User);
                await _mediator.SendAsync(new CreateInviteCommand { Invite = invite, UserId = user?.Id });

                return RedirectToAction(actionName: "Details", controllerName: "Campaign", routeValues: new { area = "Admin", id = invite.CampaignId });
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
                var @event = await _mediator.SendAsync(new EventByEventIdQuery { EventId = invite.EventId });
                if (@event == null)
                {
                    return BadRequest();
                }

                if (!IsUserAuthorizedToSendInvite(@event.Campaign.ManagingOrganizationId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.GetUserAsync(User);
                await _mediator.SendAsync(new CreateInviteCommand { Invite = invite, UserId = user?.Id });

                return RedirectToAction(actionName: "Details", controllerName: "Event", routeValues: new { area = "Admin", id = invite.EventId });
            }

            return View("Send", invite);
        }

        private bool IsUserAuthorizedToSendInvite(int organizationId)
        {
            if (!User.IsOrganizationAdmin())
            {
                return false;
            }

            var userOrganizationId = User.GetOrganizationId();

            if (!userOrganizationId.HasValue || organizationId != userOrganizationId)
            {
                return false;
            }
            return true;
        }

    }
}