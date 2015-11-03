using AllReady.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Controllers
{
    public static class ControllerExtensions
    {
        public static ViewResult WithSkills(this ViewResult view, IAllReadyDataAccess dataAccess)
        {
            return view.WithSkills(dataAccess, s => new {
                Id = s.Id,
                Name = s.HierarchicalName,
                Description = s.Description
            });
        }

        public static ViewResult WithSkills<TResult>(this ViewResult view, IAllReadyDataAccess dataAccess, Func<Skill,TResult> action)
        {
            view.ViewData["Skills"] = dataAccess.Skills
                .OrderBy(a => a.HierarchicalName)
                .Select(action)
                .ToList();
            return view;
        }
    }
}
