using MediatR;
using System;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    //TODO: reanme to SetRequestToUnassigned
    public class RemoveRequestCommand : IAsyncRequest
    {
        public Guid RequestId { get; set; }
        public int ItineraryId { get; set; }
    }
}
