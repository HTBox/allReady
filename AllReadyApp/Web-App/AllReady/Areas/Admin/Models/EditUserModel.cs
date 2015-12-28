using AllReady.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class EditUserModel
    {
        public string UserId { get; set; }
        [Display(Name = "User name")]
        public string UserName { get; set; }
        public Organization Organization { get; set; }
        [Display(Name = "Is organization admin?")]
        public bool IsOrganizationAdmin { get; set; }
        [Display(Name = "Is site admin?")]
        public bool IsSiteAdmin { get; set; }
        [Display(Name = "Associated skills")]
        public List<UserSkill> AssociatedSkills { get; set; }
    }
}
