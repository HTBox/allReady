using AllReady.Areas.Admin.Models.TaskSignupModels;
using MediatR;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class TaskSignupSummaryQuery : IAsyncRequest<TaskSignupSummaryModel>
    {
        public int TaskSignupId { get; set; }
    }
}
