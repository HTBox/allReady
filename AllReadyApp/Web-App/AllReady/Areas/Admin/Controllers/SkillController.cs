using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Skills;
using AllReady.Areas.Admin.Features.Organizations;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
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
                var allSkills = await _mediator.SendAsync(new SkillListQueryAsync());
                return View(allSkills);
            }

            var organizationId = User.GetOrganizationId();
            if (!organizationId.HasValue)
            {
                return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
            }
            
            var organizationName = await _mediator.SendAsync(new OrganizationNameQueryAsync { Id = organizationId.Value });
            if (string.IsNullOrEmpty(organizationName))
            {
                return HttpNotFound();
            }

            ViewData["Title"] = $"Skills - {organizationName}";
            var organizationSkills = await _mediator.SendAsync(new SkillListQueryAsync { OrganizationId = organizationId.Value });

            return View("Index", organizationSkills);
        }

        // GET /Admin/Skill/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var organizationId = User.GetOrganizationId();

            if (!User.IsUserType(UserType.SiteAdmin) && !organizationId.HasValue)
            {
                return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
            }
            
            var model = new SkillEditModel();

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {              
                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync { OrganizationId = organizationId.Value });
            }

            return View("Edit", model);
        }
        
        // POST /Admin/Skill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SkillEditModel model)
        {
            if (ModelState.IsValid)
            {
                if (!User.IsUserType(UserType.SiteAdmin))
                {
                    model.OwningOrganizationId = User.GetOrganizationId();
                }

                await _mediator.SendAsync(new SkillEditCommandAsync { Skill = model });

                return RedirectToAction(nameof(Index), new { area = "Admin" });
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {
                var organizationId = User.GetOrganizationId();
                if (!organizationId.HasValue)
                {
                    return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
                }
                
                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync { OrganizationId = organizationId.Value });
            }

            return View("Edit", model);
        }
        
        // GET /Admin/Skill/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _mediator.SendAsync(new SkillEditQueryAsync { Id = id });
            if (model == null)
            {
                return HttpNotFound();
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {
                var organizationId = User.GetOrganizationId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!organizationId.HasValue || model.OwningOrganizationId != organizationId)
                {
                    return new HttpUnauthorizedResult();
                }
                
                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync { OrganizationId = organizationId.Value });
            }

            model.ParentSelection = model.ParentSelection.Where(p => p.Id != model.Id); // remove self from the parent select list

            return View("Edit", model);
        }

        // POST /Admin/Skill/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SkillEditModel model)
        {
            if (ModelState.IsValid)
            {
                await _mediator.SendAsync(new SkillEditCommandAsync { Skill = model });
                return RedirectToAction(nameof(Index), new { area = "Admin" });
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _mediator.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {
                var organizationId = User.GetOrganizationId();
                if (!organizationId.HasValue)
                {
                    return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim
                }

                model.ParentSelection = await _mediator.SendAsync(new SkillListQueryAsync { OrganizationId = organizationId.Value });
            }

            model.ParentSelection = model.ParentSelection.Where(p => p.Id != model.Id); // remove self from the parent select list

            return View(model);
        }

        // GET /Admin/Skill/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _mediator.SendAsync(new SkillDeleteQueryAsync { Id = id });
            if (model == null)
            {
                return HttpNotFound();
            }

            // security check to ensure the skill belongs to the same org as the org admin
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                var organizationId = User.GetOrganizationId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!organizationId.HasValue || model.OwningOrganizationId != organizationId)
                {
                    return new HttpUnauthorizedResult();
                }
            }

            return View("Delete", model);
        }

        // GET /Admin/Skill/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _mediator.SendAsync(new SkillDeleteQueryAsync { Id = id });
            if (model == null)
            {
                return HttpNotFound();
            }

            // security check to ensure the skill belongs to the same org as the org admin
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                var organizationId = User.GetOrganizationId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!organizationId.HasValue || model.OwningOrganizationId != organizationId)
                {
                    return new HttpUnauthorizedResult();
                }
            }

            await _mediator.SendAsync(new SkillDeleteCommandAsync { Id = id });
            return RedirectToAction(nameof(Index), new { area = "Admin" });
        }
    }
}