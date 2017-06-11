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

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(nameof(UserType.OrgAdmin))]
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
        public async Task<IActionResult> Send(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignByCampaignIdQuery { CampaignId = campaignId });
            if (campaign == null) return NotFound();

            if (!User.IsOrganizationAdmin(campaign.ManagingOrganizationId))
            {
                return Unauthorized();
            }

            return View("Send", new CampaignManagerInviteViewModel
            {
                CampaignId = campaignId,
                CampaignName = campaign.Name,
            });
        }

        // POST: Admin/Invite/Send/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

                if (campaign == null)
                {
                    return BadRequest();
                }

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
                if (userToInvite != null && userToInvite.ManagedCampaigns.Any(m => m.CampaignId == campaignId))
                {
                    ModelState.AddModelError("InviteeEmailAddress", $"{invite.InviteeEmailAddress} is allready a manager for this campaign");
                    return View("Send", invite);
                }

                invite.CampaignId = campaignId;
                var user = await _userManager.GetUserAsync(User);
                await _mediator.SendAsync(new CreateCampaignManagerInviteCommand { Invite = invite, UserId = user?.Id });

                return RedirectToAction(actionName: "Details", controllerName: "Campaign", routeValues: new { area = "Admin", id = invite.CampaignId });
            }

            return View("Send", invite);
        }

        [Route("[area]/[controller]/[action]/{inviteId:int}")]
        public async Task<IActionResult> Details(int inviteId)
        {
            CampaignManagerInviteDetailsViewModel viewModel = await _mediator.SendAsync(new CampaignManagerInviteDetailQuery { CampaignManagerInviteId = inviteId });

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