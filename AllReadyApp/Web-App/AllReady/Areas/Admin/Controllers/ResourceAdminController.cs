using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Resource;
using AllReady.Security;
using AllReady.ViewModels.Resource;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class ResourceAdminController : Controller
    {
        private readonly IMediator _mediator;

        public ResourceAdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("Admin/Resource/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            var viewModel = new ResourceCreateViewModel
            {
                CampaignId = campaign.Id
            };

            return View(viewModel);
        }
    }
}
