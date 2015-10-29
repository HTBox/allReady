using AllReady.Models;
using AllReady.Security;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class SkillController : Controller
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public SkillController(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        private ViewResult WithSkills(ViewResult view)
        {
            view.ViewData["Skills"] = _dataAccess.Skills;
            return view;
        }

        public IActionResult Index()
        {
            return View(_dataAccess.Skills);
        }

        public IActionResult Create()
        {
            return WithSkills(View("Edit"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Skill skill)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                _dataAccess.AddSkill(skill);
                return RedirectToAction("Index", new { area = "Admin" });
            }

            return WithSkills(View("Edit", skill));
        }

        public IActionResult Edit(int? id)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return new HttpUnauthorizedResult();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(404);
            }
            Skill skill = _dataAccess.GetSkill((int)id);
            if (skill == null)
            {
                return new HttpStatusCodeResult(404);
            }

            return WithSkills(View(skill));
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Skill skill)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                _dataAccess.UpdateSkill(skill);
                return RedirectToAction("Index", new { area = "Admin" });
            }
            return WithSkills(View(skill));
        }

        // GET: Campaign/Delete/5
        public IActionResult Delete(int? skillId)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return new HttpUnauthorizedResult();
            }

            if (skillId == null)
            {
                return new HttpStatusCodeResult(404);
            }
            Skill skill = _dataAccess.GetSkill((int)skillId);
            if (skill == null)
            {
                return new HttpStatusCodeResult(404);
            }

            return View(skill);
        }

        // POST: Campaign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int skillId)
        {
            if (!User.IsUserType(UserType.SiteAdmin))
            {
                return new HttpUnauthorizedResult();
            }

            _dataAccess.DeleteSkill(skillId);
            return RedirectToAction("Index", new { area = "Admin" });
        }

    }
}
