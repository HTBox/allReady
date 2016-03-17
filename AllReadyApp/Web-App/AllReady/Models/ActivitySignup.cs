using System;

namespace AllReady.Models
{
    public class ActivitySignup
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string PreferredEmail { get; set; }
        public string PreferredPhoneNumber { get; set; }
        public string AdditionalInfo { get; set; }
        public Activity Activity { get; set; }
        public DateTime SignupDateTime { get; set; }
        public DateTime? CheckinDateTime { get; set; }
    }
}
