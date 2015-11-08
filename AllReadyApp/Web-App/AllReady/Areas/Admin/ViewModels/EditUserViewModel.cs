using AllReady.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.ViewModels
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "User name")]
        public string UserName { get; set; }
        public Tenant Tenant { get; set; }
        [Display(Name = "Is organization admin?")]
        public bool IsTenantAdmin { get; set; }
        [Display(Name = "Is site admin?")]
        public bool IsSiteAdmin { get; set; }
        [Display(Name = "Associated skills")]
        public List<UserSkill> AssociatedSkills { get; set; }
    }
}
