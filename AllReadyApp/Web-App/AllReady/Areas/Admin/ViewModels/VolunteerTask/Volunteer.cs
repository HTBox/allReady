using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.VolunteerTask
{
    /// <summary>
    ///     Short information about a volunteer
    /// </summary>
    public class VolunteerSummary
    {
        /// <summary>
        ///     Number of tasks that this user has volunteered for.
        /// </summary>
        [Display(Name = "Task count")]
        public int TaskCount { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }
    }
}
