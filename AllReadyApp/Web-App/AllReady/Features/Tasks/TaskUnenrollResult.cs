using AllReady.Models;

namespace AllReady.Features.Tasks
{
    public class TaskUnenrollResult
    {
        public string Status { get; set; }
        public VolunteerTask Task { get; set; }
    }
}
