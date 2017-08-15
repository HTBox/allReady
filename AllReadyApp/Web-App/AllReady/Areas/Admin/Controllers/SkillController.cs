using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Skills;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.ViewModels.Skill;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AllReady.Constants;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize(nameof(UserType.OrgAdmin))]
    public class SkillController : Controller
    {
        private readonly IMediator _mediator;

        public SkillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /Admin/Skill
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsUserType(UserType.SiteAdmin))
            {
                ViewData["Title"] = "Skills";
                var allSkills = await _mediator.SendAsync(new SkillListQuery());
                return View(allSkills);
            }

            var organizationId = User.GetOrganizationId();
            if (!organizationId.HasValue)
            {
                return new UnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
            }
            
            var organizationName = await _mediator.SendAsync(new OrganizationNameQuery { Id = organizationId.Value });
            if (string.IsNullOrEmpty(organizationName))
            {
                return NotFound();
            }

            ViewData["Title"] = $"Skills - {organizationName}";
            var organizationSkills = await _mediator.SendAsync(new SkillListQuery { OrganizationId = organizationId.Value });

            return View("Index", organizationSkills);
        }

        // GET /Admin/Skill/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var organizationId = User.GetOrganizationId();

            if (!User.IsUserType(UserType.SiteAdmin) && !organizationId.HasValue)
            {
                return new UnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
            }
            
            var model = new SkillEditViewModel();

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQuery());
            }
            else
            {              
                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery { OrganizationId = organizationId.Value });
            }

            return View("Edit", model);
        }
        
        // POST /Admin/Skill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SkillEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!User.IsUserType(UserType.SiteAdmin))
                {
                    model.OwningOrganizationId = User.GetOrganizationId();
                }

                await _mediator.SendAsync(new SkillEditCommand { Skill = model });

                return RedirectToAction(nameof(Index), new { area = AreaNames.Admin });
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQuery());
            }
            else
            {
                var organizationId = User.GetOrganizationId();
                if (!organizationId.HasValue)
                {
                    return new UnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
                }
                
                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery { OrganizationId = organizationId.Value });
            }

            return View("Edit", model);
        }
        
        // GET /Admin/Skill/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _mediator.SendAsync(new SkillEditQuery { Id = id });
            if (model == null)
            {
                return NotFound();
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQuery());
            }
            else
            {
                var organizationId = User.GetOrganizationId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!organizationId.HasValue || model.OwningOrganizationId != organizationId)
                {
                    return new UnauthorizedResult();
                }
                
                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery { OrganizationId = organizationId.Value });
            }

            var descendants = model.ParentSelection.SingleOrDefault(x => x.Id == id)?.DescendantIds ?? new List<int>();

            model.ParentSelection = model.ParentSelection.Where(p => p.Id != model.Id); // remove self from the parent select list
            model.ParentSelection = model.ParentSelection.Where(p => !descendants.Contains(p.Id)); // remove any descendants from the parent selection list to avoid hierarchical loops

            return View("Edit", model);
        }

        // POST /Admin/Skill/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SkillEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _mediator.SendAsync(new SkillEditCommand { Skill = model });
                return RedirectToAction(nameof(Index), new { area = AreaNames.Admin });
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQuery());
            }
            else
            {
                var organizationId = User.GetOrganizationId();
                if (!organizationId.HasValue)
                {
                    return new UnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
                }

                model.ParentSelection = await _mediator.SendAsync(new SkillListQuery { OrganizationId = organizationId.Value });
            }

            model.ParentSelection = model.ParentSelection.Where(p => p.Id != model.Id); // remove self from the parent select list

            return View(model);
        }

        // GET /Admin/Skill/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _mediator.SendAsync(new SkillDeleteQuery { Id = id });
            if (viewModel == null)
            {
                return NotFound();
            }

            // security check to ensure the skill belongs to the same org as the org admin
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                var organizationId = User.GetOrganizationId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!organizationId.HasValue || viewModel.OwningOrganizationId != organizationId)
                {
                    return new UnauthorizedResult();
                }
            }

            viewModel.Title = $"Delete skill {viewModel.HierarchicalName}";
            viewModel.SkillBelongsToSameOrgAsOrgAdmin = true;

            return View("Delete", viewModel);
        }

        // GET /Admin/Skill/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(SkillDeleteViewModel viewModel)
        {
            if (!viewModel.SkillBelongsToSameOrgAsOrgAdmin)
            {
                return new UnauthorizedResult();
            }

            await _mediator.SendAsync(new SkillDeleteCommand { Id = viewModel.SkillId });

            return RedirectToAction(nameof(Index), new { area = AreaNames.Admin });
        }
    }
}