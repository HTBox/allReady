using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class DeleteOrganization : IAsyncRequest
    {
        public int Id { get; set; }
    }
}
