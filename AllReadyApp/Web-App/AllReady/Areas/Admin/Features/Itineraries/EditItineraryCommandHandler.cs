using System;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class EditItineraryCommandHandler : IAsyncRequestHandler<EditItineraryCommand, int>
    {
        private readonly AllReadyContext _context;

        public EditItineraryCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditItineraryCommand message)
        {
            try
            {
                var itinerary = await GetItinerary(message) ?? _context.Add(new Itinerary()).Entity;

                itinerary.Name = message.Itinerary.Name;
                itinerary.Date = message.Itinerary.Date;
                itinerary.EventId = message.Itinerary.EventId;
                
                await _context.SaveChangesAsync();

                return itinerary.Id;
            }
            catch (Exception)
            {
                // There was an error somewhere
                return 0;
            }
        }

        private async Task<Itinerary> GetItinerary(EditItineraryCommand message)
        {
            return await _context.Itineraries
                .SingleOrDefaultAsync(c => c.Id == message.Itinerary.Id);
        }
    }
}