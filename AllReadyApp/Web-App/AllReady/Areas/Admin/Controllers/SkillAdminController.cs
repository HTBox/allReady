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
            if (ModelState.IsValid)
            {
                _dataAccess.AddSkill(skill);
                return RedirectToAction("Index", new { area = "Admin" });
            }

            return WithSkills(View("Edit", skill));
        }

        public IActionResult Edit(int id)
        {
            Skill skill = _dataAccess.GetSkill(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            return WithSkills(View(skill));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Skill skill)
        {
            if (ModelState.IsValid)
            {
                _dataAccess.UpdateSkill(skill);
                return RedirectToAction("Index", new { area = "Admin" });
            }
            return WithSkills(View(skill));
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
            _dataAccess.DeleteSkill(id);
            return RedirectToAction("Index", new { area = "Admin" });
        }

    }
}
