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
            view.ViewData["Skills"] = dataAccess.Skills
                .OrderBy(a => a.HierarchicalName)
                .ToList();
            return view;
        }
    }
}
