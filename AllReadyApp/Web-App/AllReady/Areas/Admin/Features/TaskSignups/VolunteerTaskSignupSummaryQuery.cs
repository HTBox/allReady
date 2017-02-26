using AllReady.Areas.Admin.ViewModels.Itinerary;
using MediatR;

namespace AllReady.Areas.Admin.Features.TaskSignups
{
    public class VolunteerTaskSignupSummaryQuery : IAsyncRequest<VolunteerTaskSignupSummaryViewModel>
    {
        public int VolunteerTaskSignupId { get; set; }
    }
}
