using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using AllReady.Security;
using AllReady.Models;
using MediatR;
using Microsoft.AspNet.Http;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.Models;
using System.Threading.Tasks;
using AllReady.Services;
using AllReady.Extensions;

namespace AllReady.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class CampaignController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IMediator _bus;
        private readonly IImageService _imageService;

        public CampaignController(IMediator bus, IImageService imageService, IAllReadyDataAccess dataAccess)
        {
            _bus = bus;
            _imageService = imageService;
            _dataAccess = dataAccess;
        }

        // GET: Campaign
        public IActionResult Index()
        {
            var campaigns = _bus.Send(new CampaignListQuery());
            return View(campaigns);
        }

        public IActionResult Details(int id)
        {
            CampaignDetailModel campaign = _bus.Send(new CampaignDetailQuery { CampaignId = id });

            if (campaign == null)
            {
                return HttpNotFound();
            }

            if (!User.IsTenantAdmin(campaign.TenantId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // GET: Campaign/Create
        public IActionResult Create()
        {
            return View("Edit", new CampaignSummaryModel());
        }

        // POST: Campaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CampaignSummaryModel campaign, IFormCollection form)
        {
            if (campaign == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsTenantAdmin(campaign.TenantId))
            {
                return HttpUnauthorized();
            }

            if (ModelState.IsValid)
            {
                if (form.Files.Count > 0)
                {
                    // If the form contains a file, upload it and update the ImageUrl.
                    if (form.Files[0] != null)
                    {
                        var file = form.Files[0];

                        if (file.IsAcceptableImageContentType())
                        {
                            campaign.ImageUrl = await _imageService.UploadCampaignImageAsync(campaign.Id, campaign.TenantId, form.Files[0]);
                        }
                        else
                        {
                            ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                            return View(campaign);
                        }
                    }
                }

                int id = _bus.Send(new EditCampaignCommand { Campaign = campaign });
                return RedirectToAction("Details", new {id = id, area = "Admin" });
            }

            return View("Edit", campaign);
        }

        // GET: Campaign/Edit/5
        public IActionResult Edit(int id)
        {
            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

            if (campaign == null)
            {
                return HttpNotFound();
            }

            if (!User.IsTenantAdmin(campaign.TenantId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CampaignSummaryModel campaign, IFormCollection form)
        {
            if (campaign == null)
            {
                return HttpBadRequest();
            }

            if (!User.IsTenantAdmin(campaign.TenantId))
            {
                return HttpUnauthorized();
            }

            if (ModelState.IsValid)
            {
                if (form.Files.Count > 0)
                {
                    // If the form contains a file, upload it and update the ImageUrl.
                    if (form.Files[0] != null)
                    {
                        var file = form.Files[0];
                        
                        if (file.IsAcceptableImageContentType())
                        {
                            campaign.ImageUrl = await _imageService.UploadCampaignImageAsync(campaign.Id, campaign.TenantId, form.Files[0]);
                        }
                        else
                        {
                            ModelState.AddModelError("FileUpload", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                            return View(campaign);
                        }
                    }
                }

                int id = _bus.Send(new EditCampaignCommand { Campaign = campaign });
                return RedirectToAction("Details", new { area = "Admin", id = id });
            }
            return View(campaign);
        }        

        // GET: Campaign/Delete/5
        public IActionResult Delete(int id)
        {
            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

            if (campaign == null)
            {
                return HttpNotFound();
            }
            if (!User.IsTenantAdmin(campaign.TenantId))
            {
                return HttpUnauthorized();
            }

            return View(campaign);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            CampaignSummaryModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

            if (!User.IsTenantAdmin(campaign.TenantId))
            {
                return HttpUnauthorized();
            }
            _bus.Send(new DeleteCampaignCommand { CampaignId = id });            
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCampaignFile(int id, IFormFile file)
        {
            var campaign = _dataAccess.GetCampaign(id);

            campaign.ImageUrl = await _imageService.UploadCampaignImageAsync(campaign.Id, campaign.ManagingTenantId, file);
            await _dataAccess.UpdateCampaign(campaign);

            return RedirectToRoute(new { controller = "Campaign", Area = "Admin", action = "Edit", id = id });

        }
    }
}
