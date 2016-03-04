using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class OrganizationController : Controller
    {
        private readonly IMediator _mediator;

        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Organization
        public IActionResult Index()
        {
            var list = _mediator.Send(new OrganizationListQuery());
            return View(list);
        }

        // GET: Organization/Details/5
        public IActionResult Details(int id)
        {
            var organization = _mediator.Send(new OrganizationDetailQuery { Id = id });
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
                _mediator.Send(new OrganizationEditCommand { Organization = organization });
                return RedirectToRoute("areaRoute", new {controller = "Organization", action = "Index"});
            }

            return View("Create", organization);
        }

        // GET: Organization/Edit/5
        public IActionResult Edit(int id)
        {

            var organization = _mediator.Send(new OrganizationEditQuery { Id = id });
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
                int id = _mediator.Send(new OrganizationEditCommand { Organization = organization });
                return RedirectToAction("Details", new { id = id, area = "Admin" });
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
            var organization = _mediator.Send(new OrganizationDetailQuery { Id = id.Value });
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
            _mediator.Send(new OrganizationDeleteCommand { Id= id });
            return RedirectToAction("Index");
        }
    }
}
