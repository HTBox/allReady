using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Goal;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class GoalController : Controller
    {
        private readonly IMediator _mediator;

        public GoalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("Admin/Goal/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }
            var viewModel = new GoalEditViewModel()
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                OrganizationId = campaign.OrganizationId,
                OrganizationName = campaign.OrganizationName
            };
            
            return View("Edit", viewModel);
        }
    }
}
