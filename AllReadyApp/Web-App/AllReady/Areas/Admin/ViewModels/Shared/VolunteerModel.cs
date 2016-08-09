namespace AllReady.Areas.Admin.Models
{
    public class VolunteerModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool HasVolunteered { get; set; }
        public string Status { get; set; }
        public string PreferredEmail { get; set; }
        public string PreferredPhoneNumber { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
