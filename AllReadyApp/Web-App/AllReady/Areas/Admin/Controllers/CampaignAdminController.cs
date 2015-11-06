
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using AllReady.Security;
using AllReady.Models;
using MediatR;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels;

namespace AllReady.Controllers
{
    [Area("Admin")]
    [Authorize("TenantAdmin")]
    public class CampaignController : Controller
    {
        private IMediator _bus;

        public CampaignController( IMediator bus)
        {
            _bus = bus;
        }

        // GET: Campaign
        public IActionResult Index()
        {
            var campaigns = _bus.Send(new CampaignListQuery());
            return View(campaigns);
        }

        public IActionResult Details(int id)
        {
            CampaignDetailViewModel campaign = _bus.Send(new CampaignDetailQuery { CampaignId = id });

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
            return View("Edit");
        }

        // POST: Campaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CampaignSummaryViewModel campaign)
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
                int id = _bus.Send(new EditCampaignCommand { Campaign = campaign });
                return RedirectToAction("Details", new {id = id, area = "Admin" });
            }

            return View("Edit", campaign);
        }

        // GET: Campaign/Edit/5
        public IActionResult Edit(int id)
        {
            CampaignSummaryViewModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

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
        public IActionResult Edit(CampaignSummaryViewModel campaign)
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
                int id = _bus.Send(new EditCampaignCommand { Campaign = campaign });
                return RedirectToAction("Details", new { area = "Admin", id = id });
            }
            return View(campaign);
        }        

        // GET: Campaign/Delete/5
        public IActionResult Delete(int id)
        {
            CampaignSummaryViewModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

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
            CampaignSummaryViewModel campaign = _bus.Send(new CampaignSummaryQuery { CampaignId = id });

            if (!User.IsTenantAdmin(campaign.TenantId))
            {
                return HttpUnauthorized();
            }
            _bus.Send(new DeleteCampaignCommand { CampaignId = id });            
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
