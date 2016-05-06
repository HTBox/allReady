using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public enum EventTypes
    {
        [Display(Name = "Event Managed")]
        EventManaged = 1,

        [Display(Name = "Task Managed")]
        TaskManaged = 2,

        [Display(Name = "Itinerary Managed")]
        ItineraryManaged = 3
    }
}