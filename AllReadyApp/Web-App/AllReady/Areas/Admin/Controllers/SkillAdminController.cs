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
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IMediator _bus;

        public SkillController(IAllReadyDataAccess dataAccess, IMediator bus)
        {
            _dataAccess = dataAccess;
            _bus = bus;
        }

        // GET /Admin/Skill
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
                int? tenantId = User.GetTenantId().Value;
                if (!tenantId.HasValue) return new HttpNotFoundResult(); // Edge case of user having Org Admin claim but not Org Id claim

                string tenantName = await _bus.SendAsync(new TenantNameQueryAsync { Id = tenantId.Value });
                if (string.IsNullOrEmpty(tenantName)) return new HttpNotFoundResult();

                ViewData["Title"] = $"Skills - {tenantName}";
                var tenantSkills = await _bus.SendAsync(new TenantSkillListQueryAsync { Id = tenantId.Value });
                return View(tenantSkills);
            }
        }

        // GET /Admin/Skill/Create
        public async Task<IActionResult> Create()
        {
            // note: this check assumes an Org Admin within an assign Org should not be able to access add sills
            if (!User.IsUserType(UserType.SiteAdmin) && !User.GetTenantId().HasValue)
                return new HttpNotFoundResult(); // Edge case of user having Org Admin claim but not Org Id claim - todo discuss correct return in this instance

            var model = new SkillEditModel(); // TODO - Populate lists
            
            //model.ParentSelectOptions.AddRange

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                if (!User.IsUserType(UserType.SiteAdmin))
                {
                    skill.OwningOrganizationId = User.GetTenantId();
                }
                _dataAccess.AddSkill(skill).Wait();
                return RedirectToAction("Index", new { area = "Admin" });
            }

            return View("Edit", skill).WithSkills(_dataAccess, !User.IsUserType(UserType.SiteAdmin) ? User.GetTenantId() : null);
        }

        public IActionResult Edit(int id)
        {
            Skill skill = _dataAccess.GetSkill(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            return View(skill).WithSkills(_dataAccess, !User.IsUserType(UserType.SiteAdmin) ? User.GetTenantId() : null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Skill skill)
        {
            if (ModelState.IsValid)
            {
                _dataAccess.UpdateSkill(skill).Wait();
                return RedirectToAction("Index", new { area = "Admin" });
            }
            return View(skill).WithSkills(_dataAccess, !User.IsUserType(UserType.SiteAdmin) ? User.GetTenantId() : null);
        }

        public IActionResult Delete(int id)
        {
            Skill skill = _dataAccess.GetSkill(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            ViewBag.Children = _dataAccess.Skills.Where(s => s.ParentSkillId == id).ToList();
            return View(skill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bus.SendAsync(new SkillDeleteCommandAsync { Id = id });
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
