using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameUniqueQuery : IAsyncRequest<bool>
    {
        public string OrganizationName { get; set; }
        public int OrganizationId { get; set; }
    }
}
