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
        private readonly AllReadyContext context;
        private readonly IGeocoder geocoder;
        private readonly IMediator mediator;

        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public AddApiRequestCommandHandler(AllReadyContext context, IGeocoder geocoder, IMediator mediator)
        {
            this.context = context;
            this.geocoder = geocoder;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(AddApiRequestCommand message)
        {
            //TODO mgmccarthy: I can probably move the Request creation to the controller and put the entity on the command instead of the view model mapping code below
            var request = new Request
            {
                RequestId = NewRequestId(),
                ProviderRequestId = message.ViewModel.ProviderRequestId,
                ProviderData = message.ViewModel.ProviderData,
                Address = message.ViewModel.Address,
                City = message.ViewModel.City,
                DateAdded = DateTimeUtcNow(),
                Email = message.ViewModel.Email,
                Name = message.ViewModel.Name,
                Phone = message.ViewModel.Phone,
                State = message.ViewModel.State,
                Zip = message.ViewModel.Zip,
                Status = RequestStatus.Unassigned,
                Source = RequestSource.Api
            };

            var address = geocoder.Geocode(message.ViewModel.Address, message.ViewModel.City, message.ViewModel.State, message.ViewModel.Zip, string.Empty).FirstOrDefault();
            request.Latitude = message.ViewModel.Latitude == 0 ? address?.Coordinates.Latitude ?? 0 : message.ViewModel.Latitude;
            request.Longitude = message.ViewModel.Longitude == 0 ? address?.Coordinates.Longitude ?? 0 : message.ViewModel.Longitude;

            context.AddOrUpdate(request);
            await context.SaveChangesAsync();

            await mediator.PublishAsync(new ApiRequestAddedNotification { RequestId = request.RequestId });
        }
    }
}