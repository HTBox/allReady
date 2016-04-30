using AllReady.Models;

namespace AllReady.Features.Tasks
{
    public class TaskSignupResult
    {
        // TODO - Move to enum
        public const string FAILURE_CLOSEDTASK = "failure-taskclosed";
        public const string SUCCESS = "success";
        public const string FAILURE_EVENTNOTFOUND = "failure-eventnotfound";
        public const string FAILURE_TASKNOTFOUND = "failure-tasknotfound";

        public string Status { get; set; }
        public AllReadyTask Task { get; set; }
    }
}