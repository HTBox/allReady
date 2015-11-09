using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ViewModels
{
    public class ActivitySignupViewModel
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        public string PreferredEmail { get; set; }
        [Display(Name = "Phone")]
        [Phone]
        public string PreferredPhoneNumber { get; set; }
        [Display(Name = "Comments")]
        public string AdditionalInfo { get; set; }
        public List<int> AddSkillIds { get; set; } = new List<int>();
    }
}
