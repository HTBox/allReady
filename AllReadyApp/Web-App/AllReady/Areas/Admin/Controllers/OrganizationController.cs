using System.Linq;
using AllReady.Areas.Admin.Features.Organizations;
using MediatR;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Campaigns;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Areas.Admin.ViewModels.Validators;
using AllReady.Constants;
using AllReady.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using AllReady.Services;
using Microsoft.AspNetCore.Http;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize(nameof(UserType.SiteAdmin))]
    public class OrganizationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrganizationEditModelValidator _organizationValidator;
        private readonly IImageService _imageService;

        public OrganizationController(IMediator mediator, IOrganizationEditModelValidator validator,
            IImageService imageService)
        {
            _mediator = mediator;
            _organizationValidator = validator;
            this._imageService = imageService;
        }

        // GET: Organization
        public async Task<IActionResult> Index()
        {
            var list = await _mediator.SendAsync(new OrganizationListQuery());
            return View(list);
        }

        // GET: Organization/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var organization = await _mediator.SendAsync(new OrganizationDetailQuery { Id = id });
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
            var organization = await _mediator.SendAsync(new OrganizationEditQuery { Id = id });
            if (organization == null)
            {
                return NotFound();
            }

            return View("Edit", organization);
        }

        // POST: Organization/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrganizationEditViewModel organization, IFormFile fileUpload)
        {
            if (organization == null)
            {
                return BadRequest();
            }

            var errors = _organizationValidator.Validate(organization);
            errors.ToList().ForEach(e => ModelState.AddModelError(e.Key, e.Value));

            if (ModelState.IsValid)
            {
                var isNameUnique = await _mediator.SendAsync(new OrganizationNameUniqueQuery { OrganizationName = organization.Name, OrganizationId = organization.Id });
                if (isNameUnique)
                {
                   
                    if (fileUpload != null)
                    {
                        if (fileUpload.IsAcceptableImageContentType())
                        {
                            await UploadOrganizationLogo(organization, fileUpload);
                        }
                        else
                        {
                            ModelState.AddModelError("ImageUrl", "You must upload a valid image file for the logo (.jpg, .png, .gif)");
                            return View("Edit", organization);
                        }
                    }

                    var id = await _mediator.SendAsync(new EditOrganizationCommand { Organization = organization });

                    return RedirectToAction(nameof(Details), new { id, area = AreaNames.Admin });
                }

                ModelState.AddModelError(nameof(organization.Name), "Organization with same name already exists. Please use different name.");
            }
            
            return View("Edit", organization);
        }

        private async Task UploadOrganizationLogo(OrganizationEditViewModel organization, IFormFile fileUpload)
        {
            var existingImageUrl = organization.LogoUrl;
            var newImageUrl = await _imageService.UploadOrganizationLogoAsync(organization.Id, fileUpload);
            if (!string.IsNullOrEmpty(newImageUrl))
            {
                organization.LogoUrl = newImageUrl;
                if (existingImageUrl != null && existingImageUrl != newImageUrl)
                {
                    await _imageService.DeleteImageAsync(existingImageUrl);
                }
            }
        }

        // GET: Organization/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new DeleteQuery { OrgId = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            viewModel.Title = $"Delete {viewModel.Name}";

            return View(viewModel);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.SendAsync(new DeleteOrganization { Id = id });
            return RedirectToAction(nameof(Index), new { area = AreaNames.Admin });
        }               
    }
}
