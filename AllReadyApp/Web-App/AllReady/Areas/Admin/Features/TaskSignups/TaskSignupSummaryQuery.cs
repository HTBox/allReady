using AllReady.Areas.Admin.ViewModels.Itinerary;
using MediatR;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class TaskSignupSummaryQuery : IAsyncRequest<TaskSignupSummaryViewModel>
    {
        public int VolunteerTaskSignupId { get; set; }
    }
}
