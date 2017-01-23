using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class CreateVolunteerTaskQuery : IAsyncRequest<EditViewModel>
    {
        public int EventId { get; set; }
    }
}
