using AllReady.Models;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Areas.Admin.ViewModels.Shared;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItinerarySummaryQueryHandler : IAsyncRequestHandler<ItinerarySummaryQuery, ItinerarySummaryViewModel>
    {
        private readonly AllReadyContext _context;

        public ItinerarySummaryQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<ItinerarySummaryViewModel> Handle(ItinerarySummaryQuery message)
        {
            var itineraryDetails = await _context.Itineraries
                .AsNoTracking()
                .Include(x => x.Event)
                    .ThenInclude(x => x.Campaign)
                        .ThenInclude(x => x.ManagingOrganization)
                .Where(a => a.Id == message.ItineraryId)
                .Select(i => new ItinerarySummaryViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Date = i.Date,
                    EventId = i.EventId,
                    CampaignId = i.Event.CampaignId,
                    OrganizationId = i.Event.Campaign.ManagingOrganizationId,
                    EventSummary = new EventSummaryViewModel
                    {
                        Id = i.EventId,
                        StartDateTime = i.Event.StartDateTime,
                        EndDateTime = i.Event.EndDateTime,
                        TimeZoneId = i.Event.TimeZoneId
                    }
                })
                .SingleOrDefaultAsync();

            return itineraryDetails;
        }
    }
}
