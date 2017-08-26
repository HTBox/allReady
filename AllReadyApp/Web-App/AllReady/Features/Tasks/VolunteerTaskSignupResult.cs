using AllReady.Models;

namespace AllReady.Features.Tasks
{
    public enum TaskResultStatus : int
    {
        SUCCESS,
        FAILURE_CLOSEDTASK,
        FAILURE_EVENTNOTFOUND,
        FAILURE_TASKNOTFOUND,
    }

    public class VolunteerTaskSignupResult
    {
        public TaskResultStatus Status { get; set; }
        public VolunteerTask VolunteerTask { get; set; }
    }
}
