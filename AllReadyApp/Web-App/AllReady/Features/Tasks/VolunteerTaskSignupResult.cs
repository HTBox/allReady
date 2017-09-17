using AllReady.Models;

namespace AllReady.Features.Tasks
{
    public enum TaskResultStatus : int
    {
        Success,
        Failure_ClosedTask,
        Failure_EventNotFound,
        Failure_TaskNotFound,
    }

    public class VolunteerTaskSignupResult
    {
        public TaskResultStatus Status { get; set; }
        public VolunteerTask VolunteerTask { get; set; }
    }
}
