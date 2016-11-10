using MediatR;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddRequestsToItineraryCommand : IAsyncRequest<bool>
    {
        public int ItineraryId { get; set; }
        public List<string> RequestIdsToAdd { get; set; }
    }
}
