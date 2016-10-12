using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class DeleteQuery : IAsyncRequest<DeleteViewModel>
    {
        public int OrgId { get; set; }
    }
}
