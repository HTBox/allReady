using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.ViewModels.Shared;
using AllReady.ModelBinding;
using AllReady.Models;

namespace AllReady.Areas.Admin.ViewModels.Event
{
    public class EventEditViewModel : EventSummaryViewModel
    {
        [UIHint("Location")]
        public LocationEditViewModel Location { get; set; }

        [Display(Name = "Required Skills")]
        public IEnumerable<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();

        [Display(Name = "Campaign Start Date")]
        [AdjustToTimezone(TimeZoneIdPropertyName = nameof(TimeZoneId))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}")]
        public DateTimeOffset CampaignStartDateTime { get; set; }

        [Display(Name = "Campaign End Date")]
        [AdjustToTimezone(TimeZoneIdPropertyName = nameof(TimeZoneId))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss.fff}")]
        public DateTimeOffset CampaignEndDateTime { get; set; }
    }
}
