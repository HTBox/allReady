using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Event
{
    public class EventEditViewModel : EventSummaryViewModel
    {
        [UIHint("Location")]
        public LocationEditViewModel Location { get; set; }

        [Display(Name = "Required Skills")]
        public IEnumerable<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();
    }
}
