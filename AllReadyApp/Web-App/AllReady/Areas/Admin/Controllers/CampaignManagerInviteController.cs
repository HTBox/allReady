using AllReady.Areas.Admin.Features.CampaignManagerInvites;
using AllReady.Areas.Admin.ViewModels.ManagerInvite;
using AllReady.Features.Campaigns;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Constants;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize]
    public class CampaignManagerInviteController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public CampaignManagerInviteController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        // GET: Admin/Invite/Send/5
        [Route("[area]/[controller]/[action]/{campaignId:int}")]
        [Authorize(nameof(UserType.OrgAdmin))]
        public async Task<IActionResult> Send(int campaignId)
        {
            var viewModel = await _mediator.SendAsync(new CampaignManagerInviteQuery { CampaignId = campaignId });
            if (viewModel == null) return NotFound();

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            return View("Send", viewModel);
        }

        // POST: Admin/Invite/Send/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(nameof(UserType.OrgAdmin))]
        [Route("[area]/[controller]/[action]/{campaignId:int}")]
        public async Task<IActionResult> Send(int campaignId, CampaignManagerInviteViewModel invite)
        {
            if (invite == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var campaign = await _mediator.SendAsync(new CampaignByCampaignIdQuery { CampaignId = campaignId });

                if (!User.IsOrganizationAdmin(campaign.ManagingOrganizationId))
                {
                    return Unauthorized();
                }

                if (await _mediator.SendAsync(new UserHasCampaignManagerInviteQuery { CampaignId = invite.CampaignId, InviteeEmail = invite.InviteeEmailAddress }))
                {
                    ModelState.AddModelError("InviteeEmailAddress", $"An invite already exists for {invite.InviteeEmailAddress}.");
                    return View("Send", invite);
                }

                var userToInvite = await _userManager.FindByEmailAsync(invite.InviteeEmailAddress);
                if (userToInvite?.ManagedCampaigns != null && userToInvite.ManagedCampaigns.Any(m => m.CampaignId == campaignId))
                {
                    ModelState.AddModelError("InviteeEmailAddress", $"{invite.InviteeEmailAddress} is allready a manager for this campaign");
                    return View("Send", invite);
                }

                invite.CampaignId = campaignId;
                var user = await _userManager.GetUserAsync(User);
                var inviteId = await _mediator.SendAsync(new CreateCampaignManagerInviteCommand
                {
                    Invite = invite,
                    UserId = user.Id,
                    IsInviteeRegistered = userToInvite != null,
                    SenderName = user.Name,
                    RegisterUrl = Url.Action(nameof(AllReady.Controllers.AccountController.Register), "Account", new { area=""}, HttpContext.Request.Scheme)
                });

                return RedirectToAction("Details", "Campaign", new { area = AreaNames.Admin, id = invite.CampaignId });
            }

            return View("Send", invite);
        }

        [Route("[area]/[controller]/[action]/{inviteId:int}")]
        [Authorize(nameof(UserType.OrgAdmin))]
        public async Task<IActionResult> Details(int inviteId)
        {
            var viewModel = await _mediator.SendAsync(new CampaignManagerInviteDetailQuery { CampaignManagerInviteId = inviteId });

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

        [Route("[area]/[controller]/[action]/{inviteId:int}")]
        [Authorize(nameof(UserType.OrgAdmin))]
        public async Task<IActionResult> Cancel(int inviteId)
        {
            var query = new CampaignManagerInviteDetailQuery {CampaignManagerInviteId = inviteId};
            var viewModel = await _mediator.SendAsync(query);
            if (viewModel == null)
            {
                return NotFound();
            }

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return Unauthorized();
            }

            var cmd = new RemoveCampaignManagerInviteCommand {InviteId = inviteId};
            await _mediator.SendAsync(cmd);

            return RedirectToAction("Details", "Campaign", new { area = AreaNames.Admin, id = viewModel.CampaignId });
        }


        [Route("[area]/[controller]/[action]/{inviteId:int}", Name = "CampaignManagerInviteAcceptRoute")]
        public async Task<IActionResult> Accept(int inviteId)
        {
            var viewModel = await _mediator.SendAsync(new AcceptDeclineCampaignManagerInviteDetailQuery { CampaignManagerInviteId = inviteId });

            if (viewModel == null)
            {
                return NotFound();
            }

            // User must be the invited user
            var user = await _userManager.GetUserAsync(User);

            if (user?.Email != viewModel.InviteeEmailAddress)
            {
                return Unauthorized();
            }

            return View(viewModel);
        }

        [Route("[area]/[controller]/[action]")]
        public async Task<IActionResult> InviteAccepted(AcceptDeclineCampaignManagerInviteViewModel viewModel)
        {
            // User must be the invited user
            var user = await _userManager.GetUserAsync(User);

            if (user?.Email != viewModel.InviteeEmailAddress)
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new AcceptCampaignManagerInviteCommand
            {
                CampaignManagerInviteId = viewModel.InviteId
            });

            await _mediator.SendAsync(new CreateCampaignManagerCommand
            {
                UserId = user.Id,
                CampaignId = viewModel.CampaignId
            });

            return View(viewModel);
        }

        [Route("[area]/[controller]/[action]/{inviteId:int}", Name = "CampaignManagerInviteDeclineRoute")]
        public async Task<IActionResult> Decline(int inviteId)
        {
            var viewModel = await _mediator.SendAsync(new AcceptDeclineCampaignManagerInviteDetailQuery { CampaignManagerInviteId = inviteId });

            if (viewModel == null)
            {
                return NotFound();
            }

            // User must be the invited user
            var user = await _userManager.GetUserAsync(User);

            if (user?.Email != viewModel.InviteeEmailAddress)
            {
                return Unauthorized();
            }

            return View(viewModel);
        }

        [Route("[area]/[controller]/[action]")]
        public async Task<IActionResult> InviteDeclined(AcceptDeclineCampaignManagerInviteViewModel viewModel)
        {
            // User must be the invited user
            var user = await _userManager.GetUserAsync(User);

            if (user?.Email != viewModel.InviteeEmailAddress)
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new DeclineCampaignManagerInviteCommand
            {
                CampaignManagerInviteId = viewModel.InviteId
            });

            return View(viewModel);
        }
    }
}
