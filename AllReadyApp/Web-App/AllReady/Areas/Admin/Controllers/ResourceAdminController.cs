using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Features.Resource;
using AllReady.Areas.Admin.ViewModels.Resource;
using AllReady.Constants;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize(nameof(UserType.OrgAdmin))]
    public class ResourceController : Controller
    {
        private readonly IMediator _mediator;

        public ResourceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get: Resouce/Create
        [HttpGet]
        [Route("Admin/Resource/Create/{campaignId}")]
        public async Task<IActionResult> Create(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            var viewModel = new ResourceEditViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name
            };

            return View("Edit", viewModel);
        }

        // POST: Resouce/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Resource/Create/{campaignId}")]
        public async Task<IActionResult> Create(ResourceEditViewModel viewModel)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = viewModel.CampaignId });

            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid) return View("Edit", viewModel);

            var id = await _mediator.SendAsync(new CreateOrEditResourceCommand { Resource = viewModel });

            return RedirectToAction(nameof(CampaignController.Details), "Campaign", new { area = AreaNames.Admin, id = viewModel.CampaignId });
        }

        [HttpGet]
        [Route("Admin/Resource/Details/{resourceId}")]
        public async Task<IActionResult> Details(int resourceId)
        {
            var resource = await _mediator.SendAsync(new ResourceDetailQuery { ResourceId = resourceId });

            return View(resource);
        }

        // GET: Resource/Delete/5
        [HttpGet]
        [Route("Admin/Resource/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new DeleteResourceQuery { ResourceId = id });

            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = viewModel.CampaignId });

            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            viewModel.Name = $"Delete resource {viewModel.Name}";
            viewModel.UserIsOrgAdmin = true;

            return View(viewModel);
        }

        // POST: Resource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ResourceDeleteViewModel viewModel)
        {
            if (!viewModel.UserIsOrgAdmin)
            {
                return Unauthorized();
            }

            await _mediator.SendAsync(new DeleteResourceCommand { ResourceId = viewModel.Id });

            return RedirectToAction(nameof(CampaignController.Details), "Campaign", new { area = AreaNames.Admin, id = viewModel.CampaignId });
        }

        // Get: Resouce/Edit
        [HttpGet]
        [Route("Admin/Resource/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var resource = await _mediator.SendAsync(new ResourceDetailQuery { ResourceId = id });

            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = resource.CampaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            var viewModel = new ResourceEditViewModel
            {
                CampaignId = campaign.Id,
                CampaignName = campaign.Name,
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                ResourceUrl = resource.ResourceUrl
            };

            return View("Edit", viewModel);
        }

        // Post: Resouce/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ResourceEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = viewModel.CampaignId });
            if (campaign == null || !User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return Unauthorized();
            }

            var resourceId = await _mediator.SendAsync(new CreateOrEditResourceCommand { Resource = viewModel });

            return RedirectToAction(nameof(Details), nameof(Resource), new { resourceId });
        }
    }
}
