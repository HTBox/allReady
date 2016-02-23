using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;
using AllReady.ViewModels;

namespace AllReady.Areas.Admin.Models
{
    public class ActivityDetailModel : ActivitySummaryModel
    {
        public LocationViewModel Location { get; set; }
        public IList<TaskSummaryModel> Tasks { get; set; } = new List<TaskSummaryModel>();
        public IList<string> Volunteers { get; set; } = new List<string>();

        [Display(Name = "Required Skills")]
        public IEnumerable<ActivitySkill> RequiredSkills { get; set; } = new List<ActivitySkill>();
    }
}
