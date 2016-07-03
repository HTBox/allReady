using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public enum EventType
    {
        [Display(Name = "Rally")]
        Rally = 2,

        [Display(Name = "Itinerary")]
        Itinerary = 3
    }
}
