using System;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AllReady.Extensions;
using AllReady.Services.Mapping.GeoCoding;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class EditRequestCommandHandler : IAsyncRequestHandler<EditRequestCommand, Guid>
    {
        private readonly AllReadyContext _context;
        private readonly IGeocodeService _geocoder;

        public EditRequestCommandHandler(AllReadyContext context, IGeocodeService geocoder)
        {
            _context = context;
            _geocoder = geocoder;
        }

        public async Task<Guid> Handle(EditRequestCommand message)
        {
            var request = await _context.Requests
                .Include(l => l.Event)
                .SingleOrDefaultAsync(t => t.RequestId == message.RequestModel.Id) ?? _context.Add(new Request { Source = RequestSource.UI }).Entity;

            var addressChanged = DetermineIfAddressChanged(message, request);

            var orgId = await _context.Events.AsNoTracking()
             .Include(rec => rec.Campaign)
             .Where(rec => rec.Id == message.RequestModel.EventId)
             .Select(rec => rec.Campaign.ManagingOrganizationId)
             .FirstOrDefaultAsync();
            
            request.EventId = message.RequestModel.EventId;
            request.OrganizationId = orgId;
            request.Address = message.RequestModel.Address;
            request.City = message.RequestModel.City;
            request.Name = message.RequestModel.Name;
            request.State = message.RequestModel.State;
            request.PostalCode = message.RequestModel.PostalCode;
            request.Email = message.RequestModel.Email;
            request.Phone = message.RequestModel.Phone;
            request.Latitude = message.RequestModel.Latitude;
            request.Longitude = message.RequestModel.Longitude;
            request.Notes = message.RequestModel.Notes;
            //If lat/long not provided and we detect the address changed, then use geocoding API to get the lat/long
            if (request.Latitude == 0 && request.Longitude == 0 && addressChanged)
            {
                var coordinates = await _geocoder.GetCoordinatesFromAddress(request.Address, request.City, request.State, request.PostalCode, string.Empty);

                request.Latitude = coordinates?.Latitude ?? 0;
                request.Longitude = coordinates?.Longitude ?? 0;
            }

            await _context.SaveChangesAsync();

            return request.RequestId;
        }

        private static bool DetermineIfAddressChanged(EditRequestCommand message, Request request)
        {
            return request.Address  != message.RequestModel.Address
                || request.City     != message.RequestModel.City
                || request.State    != message.RequestModel.State
                || request.PostalCode      != message.RequestModel.PostalCode;
        }
    }
}
