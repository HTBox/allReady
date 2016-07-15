using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public class EventDetailModel : EventSummaryModel
    {
        [UIHint("Location")]
        public LocationEditModel Location { get; set; }
        public IList<TaskSummaryModel> Tasks { get; set; } = new List<TaskSummaryModel>();
        [Display(Name = "Required Skills")]
        public IEnumerable<EventSkill> RequiredSkills { get; set; } = new List<EventSkill>();

        public IEnumerable<ItineraryListModel> Itineraries { get; set; } = new List<ItineraryListModel>();

        public bool DisplayItineraries => EventType == EventType.Itinerary;

        public ItineraryEditModel NewItinerary { get; set; } = new ItineraryEditModel();

        public string ItinerariesDetailsUrl { get; set; }
    }
}
