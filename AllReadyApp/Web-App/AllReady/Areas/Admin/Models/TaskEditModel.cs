using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models
{
    public class TaskEditModel : TaskSummaryModel
    {
        [Display(Name = "Required skills")]
        public IList<TaskSkill> RequiredSkills { get; set; } = new List<TaskSkill>();
    }
}
