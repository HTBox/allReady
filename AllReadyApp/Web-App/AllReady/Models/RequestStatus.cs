using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public enum RequestStatus
    {
        [Display(Name = "Unassigned")]
        Unassigned = 0,
        [Display(Name = "Assigned")]
        Assigned = 1,
        [Display(Name = "Completed")]
        Completed = 2,
        [Display(Name = "Canceled")]
        Canceled = 3,
        [Display(Name = "Confirmed")]
        Confirmed = 4,
        [Display(Name = "Pending Confirmation")]
        PendingConfirmation = 5,
        [Display(Name = "Requested")]
        Requested = 6
    }
}
