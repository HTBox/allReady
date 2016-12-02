using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Models;
using MediatR;
using Geocoding;

namespace AllReady.Features.Requests
{
    public class ProcessApiRequestCommandHandler : AsyncRequestHandler<ProcessApiRequestCommand>
    {
        private readonly AllReadyContext context;
        private readonly IGeocoder geocoder;
        private readonly IMediator mediator;

        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public ProcessApiRequestCommandHandler(AllReadyContext context, IGeocoder geocoder, IMediator mediator)
        {
            this.context = context;
            this.geocoder = geocoder;
            this.mediator = mediator;
        }

        protected override async Task HandleCore(ProcessApiRequestCommand message)
        {        
            //TODO mgmcarthy: add OrganizationId to the Requst model and hard-code OrgId of 1 (HTBox in SampleData) here when we create the Request
            var request = new Request
            {
                RequestId = NewRequestId(),
                ProviderId = message.ViewModel.ProviderRequestId,
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

            //TODO: since GASA is not sending us Lat/Long, can we get rid of this code?
            var address = geocoder.Geocode(message.ViewModel.Address, message.ViewModel.City, message.ViewModel.State, message.ViewModel.Zip, string.Empty).FirstOrDefault();
            request.Latitude = address?.Coordinates.Latitude ?? 0;
            request.Longitude = address?.Coordinates.Longitude ?? 0;

            context.AddOrUpdate(request);
            await context.SaveChangesAsync();

            await mediator.PublishAsync(new ApiRequestProcessedNotification { ProviderRequestId = message.ViewModel.ProviderRequestId });
        }
    }
}