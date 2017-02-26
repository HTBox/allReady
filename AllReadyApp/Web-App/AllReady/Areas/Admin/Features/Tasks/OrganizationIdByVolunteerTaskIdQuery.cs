using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class OrganizationIdByVolunteerTaskIdQuery : IAsyncRequest<int>
    {
        public int VolunteerTaskId { get; set; }
    }
}