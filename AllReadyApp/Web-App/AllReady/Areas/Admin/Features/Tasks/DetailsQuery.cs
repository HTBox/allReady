using AllReady.Areas.Admin.ViewModels.VolunteerTask;

using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DetailsQuery : IAsyncRequest<DetailsViewModel>
    {
        public int VolunteerTaskId { get; set; }
    }
}
