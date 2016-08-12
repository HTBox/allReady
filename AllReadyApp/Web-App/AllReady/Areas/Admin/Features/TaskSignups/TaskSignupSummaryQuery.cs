using AllReady.Areas.Admin.ViewModels.TaskSignup;
using MediatR;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class TaskSignupSummaryQuery : IAsyncRequest<TaskSignupSummaryViewModel>
    {
        public int TaskSignupId { get; set; }
    }
}
