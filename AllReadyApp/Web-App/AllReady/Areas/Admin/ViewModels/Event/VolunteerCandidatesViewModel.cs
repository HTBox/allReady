using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AllReady.Areas.Admin.ViewModels.Event
{
    /// <summary>
    /// User that can be assigned to the given task
    /// </summary>
    public class VolunteerCandidatesViewModel
    {
        public List<SelectListItem> Volunteers { get; set; }

        /// <summary>
        /// Amount of users that may be selected (total number - already assigned count)
        /// </summary>
        public int MaxSelectableCount { get; set; }
    }
}
