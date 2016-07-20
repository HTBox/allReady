using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameUniqueQueryAsync : IAsyncRequest<bool>
    {
        public string OrganizationName { get; set; }
        public int OrganizationId { get; set; }
    }
}
