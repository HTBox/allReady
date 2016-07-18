using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class DeleteOrganizationAsync : IAsyncRequest
    {
        public int Id { get; set; }
    }
}
