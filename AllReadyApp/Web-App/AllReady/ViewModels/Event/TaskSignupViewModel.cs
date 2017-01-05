using System;
using AllReady.Models;

namespace AllReady.ViewModels.Event
{
    public class TaskSignupViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public string StatusDescription { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public TaskSignupViewModel() { }

        public TaskSignupViewModel(TaskSignup taskSignup)
        {
            Id = taskSignup.Id;
            Status = taskSignup.Status.ToString();
            StatusDateTimeUtc = taskSignup.StatusDateTimeUtc;
            StatusDescription = taskSignup.StatusDescription;

            if (taskSignup.Task != null)
            {
                TaskId = taskSignup.Task.Id;
                TaskName = taskSignup.Task.Name;
            }

            if (taskSignup.User != null)
            {
                UserId = taskSignup.User.Id;
                UserName = taskSignup.User.UserName;
            }
        }
    }
}