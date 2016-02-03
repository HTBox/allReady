using System.ComponentModel.DataAnnotations;

namespace AllReady.Models
{
    public enum ActivityTypes
    {
        [Display(Name = "Activity Managed")]
        ActivityManaged,

        [Display(Name = "Task Managed")]
        TaskManaged,

        [Display(Name = "Deployment Managed")]
        DeploymentManaged
    }
}