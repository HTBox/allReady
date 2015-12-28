using AllReady.Areas.Admin.Features.Skills;
using AllReady.Areas.Admin.Features.Tenants;
using AllReady.Areas.Admin.Models;
using AllReady.Models;
using AllReady.Security;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class SkillController : Controller
    {
        private readonly IMediator _bus;

        public SkillController(IMediator bus)
        {
            _bus = bus;
        }

        // GET /Admin/Skill
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsUserType(UserType.SiteAdmin))
            {
                ViewData["Title"] = "Skills";
                var allSkills = await _bus.SendAsync(new SkillListQueryAsync());
                return View(allSkills);
            }
            else
            {
                var tenantId = User.GetTenantId();
                if (!tenantId.HasValue)
                    return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim

                string organizationName = await _bus.SendAsync(new OrganizationNameQueryAsync { Id = tenantId.Value });
                if (string.IsNullOrEmpty(organizationName)) return HttpNotFound();

                ViewData["Title"] = $"Skills - {organizationName}";
                var tenantSkills = await _bus.SendAsync(new SkillListQueryAsync { OrganizationId = tenantId.Value });

                return View("Index", tenantSkills);
            }
        }

        // GET /Admin/Skill/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var tenantId = User.GetTenantId();

            if (!User.IsUserType(UserType.SiteAdmin) && !tenantId.HasValue)
                return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim

            var model = new SkillEditModel();

            if(User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _bus.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {              
                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync { OrganizationId = tenantId.Value });
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
                    model.OwningOrganizationId = User.GetTenantId();
                }
                await _bus.SendAsync(new SkillEditCommandAsync { Skill = model });
                return RedirectToAction("Index", new { area = "Admin" });
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _bus.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {
                var tenantId = User.GetTenantId();
                if (!tenantId.HasValue)
                    return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim

                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync { OrganizationId = tenantId.Value });
            }

            return View("Edit", model);
        }
        
        // GET /Admin/Skill/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SkillEditModel model = await _bus.SendAsync(new SkillEditQueryAsync { Id = id });

            if (model == null)
            {
                return HttpNotFound();
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _bus.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {
                var tenantId = User.GetTenantId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!tenantId.HasValue || model.OwningOrganizationId != tenantId)
                    return new HttpUnauthorizedResult();

                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync { OrganizationId = tenantId.Value });
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
                await _bus.SendAsync(new SkillEditCommandAsync { Skill = model });
                return RedirectToAction("Index", new { area = "Admin" });
            }

            if (User.IsUserType(UserType.SiteAdmin))
            {
                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync());
                model.OrganizationSelection = await _bus.SendAsync(new OrganizationSelectListQueryAsync());
            }
            else
            {
                var tenantId = User.GetTenantId();
                if (!tenantId.HasValue)
                    return new HttpUnauthorizedResult(); // Edge case of user having Org Admin claim but not Org Id claim

                model.ParentSelection = await _bus.SendAsync(new SkillListQueryAsync { OrganizationId = tenantId.Value });
            }

            model.ParentSelection = model.ParentSelection.Where(p => p.Id != model.Id); // remove self from the parent select list

            return View(model);
        }

        // GET /Admin/Skill/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            SkillDeleteModel model = await _bus.SendAsync(new SkillDeleteQueryAsync { Id = id });

            if (model == null)
            {
                return HttpNotFound();
            }

            // security check to ensure the skill belongs to the same org as the org admin
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                var tenantId = User.GetTenantId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!tenantId.HasValue || model.OwningOrganizationId != tenantId)
                    return new HttpUnauthorizedResult();
            }

            return View("Delete", model);
        }

        // GET /Admin/Skill/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SkillDeleteModel model = await _bus.SendAsync(new SkillDeleteQueryAsync { Id = id });

            if (model == null)
            {
                return HttpNotFound();
            }

            // security check to ensure the skill belongs to the same org as the org admin
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                var tenantId = User.GetTenantId();

                // security check to ensure the skill belongs to the same org as the org admin
                if (!tenantId.HasValue || model.OwningOrganizationId != tenantId)
                    return new HttpUnauthorizedResult();
            }

            await _bus.SendAsync(new SkillDeleteCommandAsync { Id = id });
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}