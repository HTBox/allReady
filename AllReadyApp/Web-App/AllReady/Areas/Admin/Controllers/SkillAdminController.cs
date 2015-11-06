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

        public IActionResult Index()
        {
            return View(_dataAccess.Skills);
        }

        public IActionResult Create()
        {
            return View("Edit").WithSkills(_dataAccess, s => s); //Append skills as-is
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                _dataAccess.AddSkill(skill).Wait();
                return RedirectToAction("Index", new { area = "Admin" });
            }

            return View("Edit", skill).WithSkills(_dataAccess, s => s);
        }

        public IActionResult Edit(int id)
        {
            Skill skill = _dataAccess.GetSkill(id);
            if (skill == null)
            {
                return HttpNotFound();
            }

            return View(skill).WithSkills(_dataAccess, s => s);
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
            return View(skill).WithSkills(_dataAccess, s => s);
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
