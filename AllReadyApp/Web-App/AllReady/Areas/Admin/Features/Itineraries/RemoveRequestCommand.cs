using MediatR;
using System;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveRequestCommand : IAsyncRequest<bool>
    {
        public Guid RequestId { get; set; }
        public int ItineraryId { get; set; }
    }
}
