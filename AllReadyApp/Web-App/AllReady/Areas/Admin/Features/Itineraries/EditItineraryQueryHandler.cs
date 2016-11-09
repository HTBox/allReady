using AllReady.Areas.Admin.ViewModels.Itinerary;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Itineraries
{
    public class EditItineraryQueryHandler : IAsyncRequestHandler<EditItineraryQuery, ItineraryEditViewModel>
    {
        private readonly AllReadyContext _context;

        public EditItineraryQueryHandler(AllReadyContext context)
        { 
            _context = context;
        }

        public async Task<ItineraryEditViewModel> Handle(EditItineraryQuery message)
        {
            var record = await _context.Itineraries.AsNoTracking()
                .Include(rec => rec.Event).ThenInclude(rec => rec.Campaign)
                .Include(rec => rec.StartLocation)
                .Include(rec => rec.EndLocation)
                .SingleOrDefaultAsync(rec => rec.Id == message.ItineraryId);

            var model = new ItineraryEditViewModel
            {
                Id = record.Id,
                Name = record.Name,
                Date = record.Date,
                EventId = record.EventId,
                OrganizationId = record.Event.Campaign.ManagingOrganizationId,
                EventName = record.Event.Name,
                CampaignId = record.Event.CampaignId,
                CampaignName = record.Event.Campaign.Name,
                UseStartAddressAsEndAddress = record.UseStartAddressAsEndAddress
            };

            if (record.StartLocation != null)
            {
                model.StartAddress1 = record.StartLocation.Address1;
                model.StartAddress2 = record.StartLocation.Address2;
                model.StartCity = record.StartLocation.City;
                model.StartState = record.StartLocation.State;
                model.StartPostalCode = record.StartLocation.PostalCode;
                model.StartCountry = record.StartLocation.Country;
            }

            if (record.EndLocation != null)
            {
                model.EndAddress1 = record.EndLocation.Address1;
                model.EndAddress2 = record.EndLocation.Address2;
                model.EndCity = record.EndLocation.City;
                model.EndState = record.EndLocation.State;
                model.EndPostalCode = record.EndLocation.PostalCode;
                model.EndCountry = record.EndLocation.Country;
            }

            return model;
        }
    }
}

