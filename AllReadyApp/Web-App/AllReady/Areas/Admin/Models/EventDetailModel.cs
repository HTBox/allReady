using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class EventDetailModel : EventSummaryModel
    {
        [UIHint("Location")]
        public LocationEditModel Location { get; set; }
        public IList<TaskSummaryModel> Tasks { get; set; } = new List<TaskSummaryModel>();
        public IList<string> Volunteers { get; set; } = new List<string>();

        [Display(Name = "Required Skills")]
        public IEnumerable<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();
    }
}
