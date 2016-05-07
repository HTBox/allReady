﻿using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class OrganizationController : Controller
    {
        private readonly IMediator _bus;
        private readonly IAllReadyDataAccess _dataAccess;
        public OrganizationController(IMediator bus, IAllReadyDataAccess dataAccess)
        {
            _bus = bus;
            _dataAccess = dataAccess;
        }


        // GET: Organization
        public IActionResult Index()
        {
            var list = _bus.Send(new OrganizationListQuery());
            return View(list);
        }

        // GET: Organization/Details/5
        public IActionResult Details(int id)
        {
            var organization = _bus.Send(new OrganizationDetailQuery { Id = id });
            if (organization == null)
            {
                return HttpNotFound();
            }

            return View(organization);
        }

        // GET: Organization/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create(OrganizationEditModel organization)
        {
            if (organization == null)
                return new BadRequestResult();

            if (ModelState.IsValid)
            {
                ValidateUniqueOrganizationName(organization);
                if (ModelState.IsValid)
                {
                    _bus.Send(new OrganizationEditCommand { Organization = organization });
                    return RedirectToRoute("areaRoute", new { controller = "Organization", action = "Index" });
                }                
            }

            return View("Create", organization);
        }

        // GET: Organization/Edit/5
        public IActionResult Edit(int id)
        {

            var organization = _bus.Send(new OrganizationEditQuery { Id = id });
            if (organization == null)
            {
                return HttpNotFound();
            }

            return View("Edit",organization);
        }

        // POST: Organization/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrganizationEditModel organization)
        {
            if (ModelState.IsValid)
            {
                var originalOrganization = _dataAccess.GetOrganization(organization.Id);
                if (originalOrganization.Name != organization.Name)
                    ValidateUniqueOrganizationName(organization);
                if (ModelState.IsValid)
                {
                    int id = _bus.Send(new OrganizationEditCommand { Organization = organization });
                    return RedirectToAction("Details", new { id = id, area = "Admin" });
                }                
            }

            return View("Edit", organization);
        }

        // GET: Organization/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            // Needs comments:  This method doesn't delete things.
            if (id == null)
            {
                return HttpNotFound();
            }
            var organization = _bus.Send(new OrganizationDetailQuery { Id = id.Value });
            if (organization == null)
            {
                return HttpNotFound();
            }

            return View(organization);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _bus.Send(new OrganizationDeleteCommand { Id= id });
            return RedirectToAction("Index");
        }

        private void ValidateUniqueOrganizationName(OrganizationEditModel organization)
        {            
            var orgs = _dataAccess.Organziations.Select(t => new OrganizationViewModel(t)).ToList();
            var existingOrgCount = orgs.Where(o => o.Name == organization.Name).ToList().Count;
            if (existingOrgCount > 0)
            {
                ModelState.AddModelError(nameof(organization.Name), "Organization with same name already exists. Please use different name.");                
            }
        }
    }
}
