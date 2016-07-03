using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class PotentialItineraryTeamMembersQuery : IAsyncRequest<IEnumerable<SelectListItem>>
    {
        public int EventId { get; set; }
        public DateTime Date { get; set; }
    }
}
