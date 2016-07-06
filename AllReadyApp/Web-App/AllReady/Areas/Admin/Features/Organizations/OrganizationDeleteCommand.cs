using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationDeleteCommand : IAsyncRequest
    {
        public int Id { get; set; }
    }
}
