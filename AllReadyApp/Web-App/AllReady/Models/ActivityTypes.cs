using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public enum ActivityTypes
    {
        [Display(Name = "Activity Managed")]
        ActivityManaged = 1,

        [Display(Name = "Task Managed")]
        TaskManaged = 2,

        [Display(Name = "Itinerary Managed")]
        ItineraryManaged = 3
    }
}