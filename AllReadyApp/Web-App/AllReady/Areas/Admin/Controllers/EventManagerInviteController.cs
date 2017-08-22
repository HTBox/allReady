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
using AllReady.Constants;
using AllReady.Areas.Admin.Features.Notifications;
namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
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
            var eventViewModel = await _mediator.SendAsync(new EventManagerInviteQuery { EventId = eventId });
            if (eventViewModel == null) return NotFound();

            if (!User.IsOrganizationAdmin(eventViewModel.OrganizationId))
            {
                return Unauthorized();
            }

            return View(eventViewModel);
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
                if (userToInvite?.ManagedEvents != null && userToInvite.ManagedEvents.Any(m => m.EventId == eventId))
                {
                    ModelState.AddModelError("InviteeEmailAddress", $"{invite.InviteeEmailAddress} is allready a manager for this event");
                    return View("Send", invite);
                }

                invite.EventId = eventId;
                var user = await _userManager.GetUserAsync(User);
                var inviteId = await _mediator.SendAsync(new CreateEventManagerInviteCommand { Invite = invite, UserId = user.Id });

                await _mediator.PublishAsync(new SendEventManagerInvite
                {
                    InviteeEmail = invite.InviteeEmailAddress,
                    EventName = invite.EventName,
                    SenderName = user.Name,
                    AcceptUrl = Url.Link("EventManagerInviteAcceptRoute", new { inviteId = inviteId }),
                    DeclineUrl = Url.Link("EventManagerInviteDeclineRoute", new { inviteId = inviteId }),
                    RegisterUrl = Url.Action(action: "Register", controller: "Account"),
                    IsInviteeRegistered = userToInvite != null,
                    Message = invite.CustomMessage
                });

                return RedirectToAction(actionName: "Details", controllerName: "Event", routeValues: new { area = AreaNames.Admin, id = invite.EventId });
            }
            return View("Send", invite);
        }

        [Route("[area]/[controller]/[action]/{inviteId:int}")]
        public async Task<IActionResult> Details(int inviteId)
        {
            var viewModel = await _mediator.SendAsync(new EventManagerInviteDetailQuery { EventManagerInviteId = inviteId });

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

        [Route("[area]/[controller]/[action]/{inviteId:int}", Name = "EventManagerInviteAcceptRoute")]
        public async Task<IActionResult> Accept(int inviteId)
        {
            // Implement in Issue 1891
            return View();
        }

        [Route("[area]/[controller]/[action]/{inviteId:int}", Name = "EventManagerInviteDeclineRoute")]
        public async Task<IActionResult> Decline(int inviteId)
        {
            // Implement in Issue 1891
            return View();
        }
    }
}
