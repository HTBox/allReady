using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationIdQuery : IAsyncRequest<int>
    {
        public int? ItineraryId { get; set; }
    }
}
