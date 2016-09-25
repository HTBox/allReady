using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class OrganizationIdByTaskIdQuery : IAsyncRequest<int>
    {
        public int TaskId { get; set; }
    }
}