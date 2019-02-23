using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Extensions;
using AllReady.Models;
using AllReady.Security;
using AllReady.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Constants;
using AllReady.ViewModels.Campaign;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize]
    public class CampaignController : Controller
    {
        public Func<DateTime> DateTimeNow = () => DateTime.Now;

        private readonly IMediator _mediator;
        private readonly IImageService _imageService;
        private readonly IImageSizeValidator _imageSizeValidator;

        public CampaignController(IMediator mediator, IImageService imageService, IImageSizeValidator imageSizeValidator)
        {
            _mediator = mediator;
            _imageService = imageService;
            _imageSizeValidator = imageSizeValidator;
        }

        // GET: Campaign
        [Authorize(nameof(UserType.OrgAdmin))]
        public async Task<IActionResult> Index()
        {
            var query = new IndexQuery();
            if (User.IsUserType(UserType.OrgAdmin))
            {
                query.OrganizationId = User.GetOrganizationId();
            }

            return View(await _mediator.SendAsync(query));
        }

        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _mediator.SendAsync(new CampaignDetailQuery { CampaignId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(viewModel.Id));
            if (!await authorizableCampaign.UserCanView())
            {
                return new ForbidResult();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult CampaignPreview(CampaignSummaryViewModel campaign, IFormFile fileUpload)
        {
            return PartialView("_CampaignPreview", new CampaignViewModel(campaign));
        }

        // GET: Campaign/Create
        [Authorize(nameof(UserType.OrgAdmin))]
        public IActionResult Create()
        {
            return View("Edit", new CampaignSummaryViewModel
            {
                StartDate = DateTimeNow(),
                EndDate = DateTimeNow().AddMonths(1),
                TimeZoneId = User.GetTimeZoneId()
            });
        }

        // GET: Campaign/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(viewModel.Id));
            if (!await authorizableCampaign.UserCanView())
            {
                return new ForbidResult();
            }

            return View(viewModel);
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CampaignSummaryViewModel campaign, IFormFile fileUpload)
        {
            if (campaign == null)
            {
                return BadRequest();
            }

            if (campaign.Id == 0)
            {
                if (!User.IsOrganizationAdmin(campaign.OrganizationId))
                {
                    return new ForbidResult();
                }
            }
            else
            {
                var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(campaign.Id));
                if (!await authorizableCampaign.UserCanEdit())
                {
                    return new ForbidResult();
                }
            }

            if (campaign.EndDate < campaign.StartDate)
            {
                ModelState.AddModelError(nameof(campaign.EndDate), "The end date must fall on or after the start date.");
            }

            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    if (!fileUpload.IsAcceptableImageContentType())
                    {
                        ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                        return View(campaign);
                    }

                    if (_imageSizeValidator != null && fileUpload.Length > _imageSizeValidator.FileSizeInBytes)
                    {
                        ModelState.AddModelError("ImageUrl", $"File size must be less than {_imageSizeValidator.BytesToMb():#,##0.00}MB!");
                        return View(campaign);
                    }

                    var existingImageUrl = campaign.ImageUrl;
                    var newImageUrl = await _imageService.UploadCampaignImageAsync(campaign.OrganizationId, campaign.Id, fileUpload);
                    if (!string.IsNullOrEmpty(newImageUrl))
                    {
                        campaign.ImageUrl = newImageUrl;
                        if (existingImageUrl != null && existingImageUrl != newImageUrl)
                        {
                            await _imageService.DeleteImageAsync(existingImageUrl);
                        }
                    }
                }

                var id = await _mediator.SendAsync(new EditCampaignCommand { Campaign = campaign });

                return RedirectToAction(nameof(Details), new { area = AreaNames.Admin, id });
            }

            return View(campaign);
        }

        // GET: Campaign/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new DeleteViewModelQuery { CampaignId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            var authorizableOrganization = await _mediator.SendAsync(new Features.Organizations.AuthorizableOrganizationQuery(viewModel.OrganizationId));
            if (!await authorizableOrganization.UserCanDelete())
            {
                return new ForbidResult();
            }

            viewModel.Title = $"Delete campaign {viewModel.Name}";

            return View(viewModel);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viewModel = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = id });
            var authorizableOrganization = await _mediator.SendAsync(new Features.Organizations.AuthorizableOrganizationQuery(viewModel.OrganizationId));
            if (!await authorizableOrganization.UserCanDelete())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new DeleteCampaignCommand { CampaignId = id });

            return RedirectToAction(nameof(Index), new { area = AreaNames.Admin });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteCampaignImage(int campaignId)
        {
            var campaign = await _mediator.SendAsync(new CampaignSummaryQuery { CampaignId = campaignId });

            if (campaign == null)
            {
                return Json(new { status = "NotFound" });
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(campaign.Id));
            if (!await authorizableCampaign.UserCanEdit())
            {
                return Json(new { status = "Unauthorized" });
            }

            if (campaign.EndDate < campaign.StartDate)
            {
                return Json(new { status = "DateInvalid", message = "The end date must fall on or after the start date." });
            }

            if (campaign.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(campaign.ImageUrl);
                campaign.ImageUrl = null;
                await _mediator.SendAsync(new EditCampaignCommand { Campaign = campaign });
                return Json(new { status = "Success" });
            }

            return Json(new { status = "NothingToDelete" });
        }


        public async Task<IActionResult> Publish(int id)
        {
            var viewModel = await _mediator.SendAsync(new PublishViewModelQuery { CampaignId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(viewModel.Id));
            if (!await authorizableCampaign.UserCanView())
            {
                return new ForbidResult();
            }

            viewModel.Title = $"Publish campaign {viewModel.Name}";
            viewModel.UserIsOrgAdmin = true;

            return View(viewModel);
        }

        // POST: Campaign/Publish/5
        [HttpPost, ActionName("Publish")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishConfirmed(PublishViewModel viewModel)
        {
            var authorizableCampaign = await _mediator.SendAsync(new AuthorizableCampaignQuery(viewModel.Id));
            if (!await authorizableCampaign.UserCanEdit())
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new PublishCampaignCommand { CampaignId = viewModel.Id });

            return RedirectToAction(nameof(Index), new { area = AreaNames.Admin });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(nameof(UserType.OrgAdmin))]
        public async Task<IActionResult> LockUnlock(int id)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return new ForbidResult();
            }

            await _mediator.SendAsync(new LockUnlockCampaignCommand { CampaignId = id });

            return RedirectToAction(nameof(Details), new { area = AreaNames.Admin, id });
        }
    }
}
