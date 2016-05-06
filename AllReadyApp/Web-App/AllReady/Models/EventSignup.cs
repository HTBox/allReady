using System;

namespace AllReady.Models
{
    public class EventSignup
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string PreferredEmail { get; set; }
        public string PreferredPhoneNumber { get; set; }
        public string AdditionalInfo { get; set; }
        public Event Event { get; set; }
        public DateTime SignupDateTime { get; set; }
        public DateTime? CheckinDateTime { get; set; }
    }
}
