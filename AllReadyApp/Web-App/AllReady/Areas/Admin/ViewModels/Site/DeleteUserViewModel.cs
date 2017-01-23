using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Site
{
    public class DeleteUserViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "User name")]
        public string UserName { get; set; }
        public bool IsSiteAdmin { get; set; }
        [Display(Name = "Organization")]
        public string OrganizationName { get; set; }
        public bool IsOrganizationAdmin { get; set;}
        [Display(Name = "Campaigns organized by user")]
        public IEnumerable<string> Campaigns { get; set; }
        [Display(Name = "Events organized by user")]
        public IEnumerable<string> Events { get; set; }
        [Display(Name = "Tasks volunteered by user")]
        public IEnumerable<string> VolunteerTasks { get; set; }
    }
}