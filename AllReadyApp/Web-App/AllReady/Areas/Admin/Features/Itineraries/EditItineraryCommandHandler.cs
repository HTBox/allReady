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

                if (itinerary.EventId == 0)
                {
                    itinerary.EventId = message.Itinerary.EventId;
                }

                if (!string.IsNullOrWhiteSpace(message.Itinerary.StartAddress1))
                {
                    var startLocation = itinerary.StartLocation ?? _context.Add(new Location()).Entity;
                    startLocation.Address1 = message.Itinerary.StartAddress1;
                    startLocation.Address2 = message.Itinerary.StartAddress2;
                    startLocation.City = message.Itinerary.StartCity;
                    startLocation.State = message.Itinerary.StartState;
                    startLocation.PostalCode = message.Itinerary.StartPostalCode;
                    startLocation.Country = message.Itinerary.StartCountry;
                    itinerary.StartLocation = startLocation;

                    if (message.Itinerary.UseStartAddressAsEndAddress)
                    {
                        if (itinerary.EndLocation != null)
                        {
                            _context.Locations.Remove(itinerary.EndLocation);
                        }

                        itinerary.EndLocation = startLocation;
                    }
                    else
                    {
                        if (itinerary.EndLocation == itinerary.StartLocation)
                        {
                            itinerary.EndLocation = null;
                        }

                        if (!string.IsNullOrWhiteSpace(message.Itinerary.EndAddress1))
                        {
                            var endLocation = itinerary.EndLocation ?? _context.Add(new Location()).Entity;
                            endLocation.Address1 = message.Itinerary.EndAddress1;
                            endLocation.Address2 = message.Itinerary.EndAddress2;
                            endLocation.City = message.Itinerary.EndCity;
                            endLocation.State = message.Itinerary.EndState;
                            endLocation.PostalCode = message.Itinerary.EndPostalCode;
                            endLocation.Country = message.Itinerary.EndCountry;
                            itinerary.EndLocation = endLocation;
                        }
                    }
                }
                else
                {
                    if (itinerary.StartLocation != null)
                    {
                        // remove the existing start location
                        _context.Locations.Remove(itinerary.StartLocation);
                        itinerary.StartLocation = null;
                    }

                    if (itinerary.EndLocation != null)
                    {
                        // remove the existing end location
                        _context.Locations.Remove(itinerary.EndLocation);
                        itinerary.EndLocation = null;
                    }
                }

                itinerary.UseStartAddressAsEndAddress = message.Itinerary.UseStartAddressAsEndAddress;

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
                .Include(rec => rec.StartLocation)
                .Include(rec => rec.EndLocation)
                .SingleOrDefaultAsync(c => c.Id == message.Itinerary.Id);
        }
    }
}