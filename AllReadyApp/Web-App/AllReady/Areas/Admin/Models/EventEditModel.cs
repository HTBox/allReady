using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class EventEditModel : EventSummaryModel
    {
        [UIHint("Location")]
        public LocationEditModel Location { get; set; }

        [Display(Name = "Required Skills")]
        public IEnumerable<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();
    }
}
