using System;
using AllReady.Models;

namespace AllReady.ViewModels.Event
{
    public class VolunteerTaskSignupViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public string StatusDescription { get; set; }
        public int VolunteerTaskId { get; set; }
        public string VolunteerTaskName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public VolunteerTaskSignupViewModel() { }

        public VolunteerTaskSignupViewModel(VolunteerTaskSignup volunteerTaskSignup)
        {
            Id = volunteerTaskSignup.Id;
            Status = volunteerTaskSignup.Status.ToString();
            StatusDateTimeUtc = volunteerTaskSignup.StatusDateTimeUtc;
            StatusDescription = volunteerTaskSignup.StatusDescription;

            if (volunteerTaskSignup.VolunteerTask != null)
            {
                VolunteerTaskId = volunteerTaskSignup.VolunteerTask.Id;
                VolunteerTaskName = volunteerTaskSignup.VolunteerTask.Name;
            }

            if (volunteerTaskSignup.User != null)
            {
                UserId = volunteerTaskSignup.User.Id;
                UserName = volunteerTaskSignup.User.UserName;
            }
        }
    }
}