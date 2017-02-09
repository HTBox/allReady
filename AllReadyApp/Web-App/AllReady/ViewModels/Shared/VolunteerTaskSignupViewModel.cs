using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Shared
{
    public class VolunteerTaskSignupViewModel
    {
        public int EventId { get; set; }

        public int VolunteerTaskId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        [Display(Name = "Comments")]
        public string AdditionalInfo { get; set; }

        public List<int> AddSkillIds { get; set; } = new List<int>();
    }
}
