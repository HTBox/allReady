using System;
using System.Linq;
using AllReady.Features.Requests;
using AllReady.Models;
using AllReady.ViewModels.Requests;
using Geocoding;
using MediatR;

namespace AllReady.Hangfire.Jobs
{
    public class ProcessApiRequests : IProcessApiRequests
    {
        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext context;
        private readonly IMediator mediator;
        private readonly IGeocoder geocoder;

        public ProcessApiRequests(AllReadyContext context, IMediator mediator, IGeocoder geocoder)
        {
            this.context = context;
            this.mediator = mediator;
            this.geocoder = geocoder;
        }

        public void Process(RequestApiViewModel viewModel)
        {
            //since this is job code now, it needs to be idempotent, this could be re-tried by Hangire if it fails
            var requestExists = context.Requests.Any(x => x.ProviderRequestId == viewModel.ProviderRequestId);
            if (!requestExists)
            {
                var request = new Request
                {
                    RequestId = NewRequestId(),
                    //TODO mgmccarthy: this is hard-coded for now to 1, which is HTBox Org's Id in dev b/c SampleDataGenerator always creates it first. We'll need something more robust when we go to production.
                    OrganizationId = 1,
                    ProviderRequestId = viewModel.ProviderRequestId,
                    ProviderData = viewModel.ProviderData,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    DateAdded = DateTimeUtcNow(),
                    Email = viewModel.Email,
                    Name = viewModel.Name,
                    Phone = viewModel.Phone,
                    State = viewModel.State,
                    Zip = viewModel.Zip,
                    Status = RequestStatus.Unassigned,
                    Source = RequestSource.Api
                };


                //this is a web service call
                var address = geocoder.Geocode(viewModel.Address, viewModel.City, viewModel.State, viewModel.Zip, string.Empty).FirstOrDefault();

                request.Latitude = address?.Coordinates.Latitude ?? 0;
                request.Longitude = address?.Coordinates.Longitude ?? 0;

                context.Add(request);
                context.SaveChanges();

                mediator.Publish(new ApiRequestProcessedNotification { ProviderRequestId = viewModel.ProviderRequestId });
            }   
        }
    }

    public interface IProcessApiRequests : IHangfireJob
    {
        void Process(RequestApiViewModel viewModel);
    }
}