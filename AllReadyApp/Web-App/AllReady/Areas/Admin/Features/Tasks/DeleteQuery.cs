using AllReady.Areas.Admin.ViewModels.VolunteerTask;

using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteQuery : IAsyncRequest<DeleteViewModel>
    {
        public int VolunteerTaskId { get; set; }
    }
}
