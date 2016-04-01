using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Security;
using AllReady.Services;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using AllReady.Areas.Admin.Models.Validators;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class CampaignController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IImageService _imageService;

        public CampaignController(IMediator mediator, IImageService imageService)
        {
            _mediator = mediator;
            _imageService = imageService;
        }

        // GET: Campaign
        public IActionResult Index()
        {
            var query = new CampaignListQuery();

            if (User.IsUserType(UserType.OrgAdmin))
            {
                query.OrganizationId = User.GetOrganizationId();
            }

            var campaigns = _mediator.Send(query);

            return View(campaigns);
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _mediator.SendAsync(new CampaignDetailQuery { CampaignId = id });
            if (viewModel == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(viewModel);
        }

        // GET: Campaign/Create
        public IActionResult Create()
        {
            return View("Edit", new CampaignSummaryModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            });
        }

        // GET: Campaign/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = id }); //not covered
            if (viewModel == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(viewModel);
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CampaignSummaryModel campaign, IFormFile fileUpload)
        {
            if (campaign == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsOrganizationAdmin(campaign.OrganizationId))
            {
                return HttpUnauthorized();
            }
            
            var validator = new CampaignSummaryModelValidator(_mediator);
            var errors = await validator.Validate(campaign);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    if (fileUpload.IsAcceptableImageContentType())
                    {
                        campaign.ImageUrl = await _imageService.UploadCampaignImageAsync(campaign.OrganizationId, campaign.Id, fileUpload);
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View(campaign);
                    }
                }

                var id = _mediator.Send(new EditCampaignCommand { Campaign = campaign });

                return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
            }

            return View(campaign);
        }

        // GET: Campaign/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = id });
            if (viewModel == null)
            {
                return HttpNotFound();
            }

            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return HttpUnauthorized();
            }

            return View(viewModel);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viewModel = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = id });
            if (!User.IsOrganizationAdmin(viewModel.OrganizationId))
            {
                return HttpUnauthorized();
            }

            await _mediator.SendAsync(new DeleteCampaignCommand { CampaignId = id });

            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUnlock(int id)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return HttpUnauthorized();
            }

            await _mediator.SendAsync(new LockUnlockCampaignCommand { CampaignId = id });

            return RedirectToAction(nameof(Details), new { area = "Admin", id = id });
        }
    }
}