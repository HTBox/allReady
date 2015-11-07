using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class TaskEditViewModel : TaskSummaryViewModel
    {
        [Display(Name = "Required skills")]
        public IEnumerable<TaskSkill> RequiredSkills { get; set; }
    }
}
