using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public enum VolunteerTaskStatus
    {
        [Display(Name = "Assigned")]
        Assigned = 0,
        [Display(Name = "Accepted")]
        Accepted = 1,
        [Display(Name = "Rejected")]
        Rejected = 2,
        [Display(Name = "Completed")]
        Completed = 3,
        [Display(Name = "Can Not Complete")]
        CanNotComplete = 4
    }
}