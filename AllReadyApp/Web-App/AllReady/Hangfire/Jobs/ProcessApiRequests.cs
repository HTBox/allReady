using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Features.Requests;
using AllReady.Models;
using AllReady.Services.Mapping;
using AllReady.Services.Mapping.GeoCoding;
using AllReady.ViewModels.Requests;
using MediatR;
using Microsoft.Extensions.Options;

namespace AllReady.Hangfire.Jobs
{
    public class ProcessApiRequests : IProcessApiRequests
    {
        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext context;
        private readonly IMediator mediator;
        private readonly IGeocodeService geocoder;
        private readonly ApprovedRegionsSettings approvedRegions;

        public ProcessApiRequests(AllReadyContext context, IMediator mediator, IGeocodeService geocoder, IOptions<ApprovedRegionsSettings> approvedRegions)
        {
            this.context = context;
            this.mediator = mediator;
            this.geocoder = geocoder;
            this.approvedRegions = approvedRegions.Value;
        }

        public void Process(RequestApiViewModel viewModel)
        {
            //since this is Hangfire job code, it needs to be idempotent, this could be re-tried if there is a failure
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
                var coordinates = geocoder.GetCoordinatesFromAddress(request.Address, request.City, request.State, request.Zip, string.Empty).Result;

                request.Latitude = coordinates?.Latitude ?? 0;
                request.Longitude = coordinates?.Longitude ?? 0;

                context.Add(request);
                context.SaveChanges();

                mediator.Publish(CreateApiRequestProcesssedNotification(request));
            }
        }

        private ApiRequestProcessedNotification CreateApiRequestProcesssedNotification(Request request)
            => new ApiRequestProcessedNotification
            {
                RequestId = request.RequestId,
                Acceptance = RequestIsFromApprovedRegion(request)
            };

        private bool RequestIsFromApprovedRegion(Request request)
            => !approvedRegions.Enabled || approvedRegions.Regions.Contains(request.ProviderData);
    }

    public interface IProcessApiRequests
    {
        void Process(RequestApiViewModel viewModel);
    }
}