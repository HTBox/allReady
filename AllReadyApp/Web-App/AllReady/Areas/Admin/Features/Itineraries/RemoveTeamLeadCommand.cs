using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class RemoveTeamLeadCommand : IAsyncRequest<bool>
    {
        public RemoveTeamLeadCommand(int itineraryId)
        {
            ItineraryId = itineraryId;
        }

        public int ItineraryId { get; }
    }
}
