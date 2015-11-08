using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ViewModels
{
    public class ActivitySignupViewModel
    {
        public int ActivityId { get; set; }
        public string PreferredEmail { get; set; }
        public string PreferredPhoneNumber { get; set; }
        public string AdditionalInfo { get; set; }
        public List<int> AddSkillIds { get; set; } = new List<int>();
    }
}
