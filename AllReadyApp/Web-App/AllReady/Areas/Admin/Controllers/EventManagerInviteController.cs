using AllReady.Areas.Admin.Features.EventManagerInvites;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Features.Events;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(nameof(UserType.OrgAdmin))]
    public class EventManagerInviteController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventManagerInviteController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        // GET: Admin/Invite/Send/5
        [Route("[area]/[controller]/[action]/{eventId:int}")]
        public async Task<IActionResult> Send(int eventId)
        {
            var @event = await _mediator.SendAsync(new EventByEventIdQuery { EventId = eventId });
            if (@event == null) return NotFound();

            if (!User.IsOrganizationAdmin(@event.Campaign.ManagingOrganizationId))
            {
                return Unauthorized();
            }

            return View("Send", new EventManagerInviteViewModel
            {
                EventId = eventId,
                EventName = @event.Name,
                CampaignId = @event.CampaignId,
                CampaignName = @event.Campaign.Name,
            });
        }

        // POST: Admin/Invite/Send/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[area]/[controller]/[action]/{eventId:int}")]
        public async Task<IActionResult> Send(int eventId, EventManagerInviteViewModel invite)
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

                if (!User.IsOrganizationAdmin(@event.Campaign.ManagingOrganizationId))
                {
                    return Unauthorized();
                }

                if (await _mediator.SendAsync(new UserHasEventManagerInviteQuery { EventId = invite.EventId, InviteeEmail = invite.InviteeEmailAddress }))
                {
                    ModelState.AddModelError("InviteeEmailAddress", $"An invite already exists for {invite.InviteeEmailAddress}.");
                    return View("Send", invite);
                }

                var userToInvite = await _userManager.FindByEmailAsync(invite.InviteeEmailAddress);
                if (userToInvite != null && userToInvite.ManagedEvents.Any(m => m.EventId == eventId))
                {
                    ModelState.AddModelError("InviteeEmailAddress", $"{invite.InviteeEmailAddress} is allready a manager for this event");
                    return View("Send", invite);
                }

                invite.EventId = eventId;
                var user = await _userManager.GetUserAsync(User);
                await _mediator.SendAsync(new CreateEventManagerInviteCommand { Invite = invite, UserId = userToInvite?.Id });
                // TODO: Send invite

                return RedirectToAction(actionName: "Details", controllerName: "Event", routeValues: new { area = "Admin", id = invite.EventId });
            }
            return View("Send", invite);
        }

        [Route("[area]/[controller]/[action]/{inviteId:int}")]
        public async Task<IActionResult> Details(int inviteId)
        {
            EventManagerInviteDetailsViewModel viewModel = await _mediator.SendAsync(new EventManagerInviteDetailQuery { EventManagerInviteId = inviteId });

            if (viewModel == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            return View(viewModel);
        }
    }
}