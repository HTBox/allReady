using AllReady.Models;

namespace AllReady.ViewModels.Task
{
    public class VolunteerTaskChangeModel
    {
        public int VolunteerTaskId { get; set; }
        public string UserId { get; set; }
        public string StatusDescription { get; set; }
        public VolunteerTaskStatus Status { get; set; }
    }
}