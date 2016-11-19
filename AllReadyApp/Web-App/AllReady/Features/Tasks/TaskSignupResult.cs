using System.ComponentModel;
using AllReady.Models;

namespace AllReady.Features.Tasks
{
    public class TaskSignupResult
    {
        public SignUpResult Status { get; set; }

        public AllReadyTask Task { get; set; }
    }

	public enum SignUpResult
	{
		[Description("success")]
		Success,
		[Description("failure-taskclosed")]
		FailureClosedTask,
		[Description("failure-eventnotfound")]
		FailureEventNotFound,
		[Description("failure-tasknotfound")]
		FailureTaskNotFound
	}
}