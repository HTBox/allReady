using System.Linq;
using AllReady.Areas.Admin.Features.Organizations;
using MediatR;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Areas.Admin.ViewModels.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class OrganizationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrganizationEditModelValidator _organizationValidator;

        public OrganizationController(IMediator mediator, IOrganizationEditModelValidator validator)
        {
            _mediator = mediator;
            _organizationValidator = validator;
        }

        // GET: Organization
        public async Task<IActionResult> Index()
        {
            var list = await _mediator.SendAsync(new OrganizationListQueryAysnc());
            return View(list);
        }

        // GET: Organization/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var organization = await _mediator.SendAsync(new OrganizationDetailQueryAsync { Id = id });
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }

        // GET: Organization/Create
        public IActionResult Create()
        {
            return View("Edit");
        }

        // GET: Organization/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var organization = await _mediator.SendAsync(new OrganizationEditQueryAsync { Id = id });
            if (organization == null)
            {
                return NotFound();
            }

            return View("Edit", organization);
        }

        // POST: Organization/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrganizationEditModel organization)
        {
            if (organization == null)
            {
                return BadRequest();
            }

            var errors = _organizationValidator.Validate(organization);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                var isNameUnique = await _mediator.SendAsync(new OrganizationNameUniqueQueryAsync { OrganizationName = organization.Name, OrganizationId = organization.Id });
                if (isNameUnique)
                {
                    var id = await _mediator.SendAsync(new EditOrganizationAsync { Organization = organization });
                    return RedirectToAction(nameof(Details), new { id, area = "Admin" });
                }

                ModelState.AddModelError(nameof(organization.Name), "Organization with same name already exists. Please use different name.");
            }

            return View("Edit", organization);
        }

        // GET: Organization/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Needs comments:  This method doesn't delete things.
            if (id == null)
            {
                return NotFound();
            }

            var organization = await _mediator.SendAsync(new OrganizationDetailQueryAsync { Id = id.Value });
            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.SendAsync(new DeleteOrganizationAsync { Id= id });
            return RedirectToAction(nameof(Index));
        }               
    }
}