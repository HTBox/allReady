using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationIdByIntineraryIdQuery : IAsyncRequest<int>
    {
        public int ItineraryId { get; set; }
    }
}
