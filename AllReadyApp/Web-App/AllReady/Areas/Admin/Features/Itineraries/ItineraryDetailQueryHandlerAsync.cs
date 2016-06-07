using AllReady.Areas.Admin.Models.ItineraryModels;
using AllReady.Models;
using MediatR;
using Microsoft.Data.Entity;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class ItineraryDetailQueryHandlerAsync : IAsyncRequestHandler<ItineraryDetailQuery, ItineraryDetailsModel>
    {
        private readonly AllReadyContext _context;

        public ItineraryDetailQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<ItineraryDetailsModel> Handle(ItineraryDetailQuery message)
        {
            ItineraryDetailsModel result = null;

            var itinerary = await GetItinerary(message);

            if (itinerary != null)
            {
                result = new ItineraryDetailsModel();

                result.Id = itinerary.Id;
                result.Name = itinerary.Name;
                result.Date = itinerary.Date;
                result.EventId = itinerary.EventId;
                result.EventName = itinerary.Event.Name;
                result.OrganizationId = itinerary.Event.Campaign.ManagingOrganization.Id;
            }

            return result;
        }

        private async Task<Itinerary> GetItinerary(ItineraryDetailQuery message)
        {
            return await _context.Itineraries
                .AsNoTracking()    
                .Include(x => x.Event)           
                .Include(x => x.Event.Campaign.ManagingOrganization)
                .SingleOrDefaultAsync(a => a.Id == message.ItineraryId)
                .ConfigureAwait(false);
        }
    }
}
