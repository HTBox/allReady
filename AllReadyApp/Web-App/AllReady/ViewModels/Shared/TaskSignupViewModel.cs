using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AllReady.ViewModels.Shared
{
    public class TaskSignupViewModel
    {
        public int EventId { get; set; }

        public int TaskId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        [Display(Name = "Comments")]
        public string AdditionalInfo { get; set; }

        public List<int> AddSkillIds { get; set; } = new List<int>();
    }
}
