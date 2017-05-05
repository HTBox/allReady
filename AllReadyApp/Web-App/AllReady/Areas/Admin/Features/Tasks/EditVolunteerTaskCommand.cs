using AllReady.Areas.Admin.ViewModels.VolunteerTask;

using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class EditVolunteerTaskCommand : IAsyncRequest<int>
    {
        public EditViewModel VolunteerTask { get; set; }
    }
}
