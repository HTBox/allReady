using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class TaskEditModel : TaskSummaryModel
    {
        [Display(Name = "Required skills")]
        public IList<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();

        public bool IgnoreTimeRangeWarning { get; set; }
    }
}
