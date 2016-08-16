using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class AddRequestsCommand : IAsyncRequest<bool>
    {
        public int ItineraryId { get; set; }
        public List<string> RequestIdsToAdd { get; set; }
    }
}
