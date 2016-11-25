using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Models;
using MediatR;
using Geocoding;

namespace AllReady.Features.Requests
{
    public class AddApiRequestCommandHandler : AsyncRequestHandler<AddApiRequestCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IGeocoder _geocoder;

        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public AddApiRequestCommandHandler(AllReadyContext context, IGeocoder geocoder)
        {
            _context = context;
            _geocoder = geocoder;
        }

        protected override async Task HandleCore(AddApiRequestCommand message)
        {
            //TODO mgmccarthy: I can probably move the Request creation to the controller and put the entity on the command instead of the view model mapping code below
            var request = new Request { RequestId = NewRequestId() };
            request.ProviderId = message.ViewModel.ProviderRequestId;
            request.ProviderData = message.ViewModel.ProviderData;
            request.Address = message.ViewModel.Address;
            request.City = message.ViewModel.City;
            request.DateAdded = DateTimeUtcNow();
            request.Email = message.ViewModel.Email;
            request.Name = message.ViewModel.Name;
            request.Phone = message.ViewModel.Phone;
            request.State = message.ViewModel.State;
            request.Zip = message.ViewModel.Zip;
            request.Status = RequestStatus.Unassigned;
            request.Source = RequestSource.Api;

            var address = _geocoder.Geocode(message.ViewModel.Address, message.ViewModel.City, message.ViewModel.State, message.ViewModel.Zip, string.Empty).FirstOrDefault();
            request.Latitude = message.ViewModel.Latitude == 0 ? address?.Coordinates.Latitude ?? 0 : message.ViewModel.Latitude;
            request.Longitude = message.ViewModel.Longitude == 0 ? address?.Coordinates.Longitude ?? 0 : message.ViewModel.Longitude;

            _context.AddOrUpdate(request);

            await _context.SaveChangesAsync();

            //TODO mgmccarthy: publish notification so Steve's code can pick it up and communicate back to getasmokealarm's API

        }
    }
}