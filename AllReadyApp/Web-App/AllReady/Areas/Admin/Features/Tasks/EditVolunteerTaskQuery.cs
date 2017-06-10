using AllReady.Areas.Admin.ViewModels.VolunteerTask;

using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditVolunteerTaskQuery : IAsyncRequest<EditViewModel>
    {
        public int VolunteerTaskId { get; set; }
    }
}
