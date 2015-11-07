using AllReady.Models;
using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class ActivityDetailViewModel : ActivitySummaryViewModel
    {
        public LocationViewModel Location { get; set; }
        public IEnumerable<TaskSummaryViewModel> Tasks { get; set; } = new List<TaskSummaryViewModel>();
        public List<string> Volunteers { get; set; } = new List<string>();

        [Display(Name = "Required Skills")]
        public IEnumerable<ActivitySkill> RequiredSkills { get; set; } = new List<ActivitySkill>();
    }
}
