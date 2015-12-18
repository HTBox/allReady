using AllReady.Models;
using AllReady.Security;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("OrgAdmin")]
    public class SkillController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public SkillController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IActionResult Index()
        {
            if (User.IsUserType(UserType.SiteAdmin))
            {
                ViewData["Title"] = "Skills";
                return View(_dataAccess.Skills);
            }
            else
            {
                ViewData["Title"] = $"Skills - {_dataAccess.GetOrganization(User.GetTenantId().Value).Name}";
                return View(_dataAccess.Skills.Where(s => s.OwningOrganizationId == User.GetTenantId()));
            }
        }

        public IActionResult Create()
        {
            return View("Edit").WithSkills(_dataAccess, !User.IsUserType(UserType.SiteAdmin) ? User.GetTenantId() : null);
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
        public IActionResult DeleteConfirmed(int id)
        {
            _dataAccess.DeleteSkill(id).Wait();
            return RedirectToAction("Index", new { area = "Admin" });
        }

    }
}
