using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class DeleteVolunteerTaskCommand : IAsyncRequest
    {
        public int VolunteerTaskId {get; set;}
    }
}
