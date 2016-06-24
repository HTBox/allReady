using MediatR;
using System;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ReorderRequestCommand : IAsyncRequest<bool>
    {
        public Guid RequestId { get; set; }
        public int ItineraryId { get; set; }
        public Direction ReOrderDirection { get; set; }

        public enum Direction
        {
            Up,
            Down
        }
    }
}
