using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Configuration;
using AllReady.Models;
using AllReady.Services.Mapping.GeoCoding;
using AllReady.ViewModels.Requests;
using Hangfire;
using Microsoft.Extensions.Options;

namespace AllReady.Hangfire.Jobs
{
    public class ProcessApiRequests : IProcessApiRequests
    {
        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        private readonly AllReadyContext context;
        private readonly IGeocodeService geocoder;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly ApprovedRegionsSettings approvedRegions;

        public ProcessApiRequests(AllReadyContext context, IGeocodeService geocoder, IOptions<ApprovedRegionsSettings> approvedRegions, IBackgroundJobClient backgroundJobClient)
        {
            this.context = context;
            this.geocoder = geocoder;
            this.backgroundJobClient = backgroundJobClient;
            this.approvedRegions = approvedRegions.Value;
        }

        public async Task Process(RequestApiViewModel viewModel)
        {
            //since this is Hangfire job code, it needs to be idempotent, this could be re-tried if there is a failure
            if (context.Requests.Any(x => x.ProviderRequestId == viewModel.ProviderRequestId))
                return;

            var requestIsFromApprovedRegion = RequestIsFromApprovedRegion(viewModel.ProviderData);

            if (requestIsFromApprovedRegion)
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
                    PostalCode = viewModel.PostalCode,
                    Status = RequestStatus.Unassigned,
                    Source = RequestSource.Api
                };

                //this is a web service call
                var coordinates = await geocoder.GetCoordinatesFromAddress(request.Address, request.City, request.State, request.PostalCode, string.Empty);

                request.Latitude = coordinates?.Latitude ?? 0;
                request.Longitude = coordinates?.Longitude ?? 0;

                context.Add(request);
                context.SaveChanges();
            }

            //acceptance is true if we can service the Request or false if can't service it 
            backgroundJobClient.Enqueue<ISendRequestStatusToGetASmokeAlarm>(x => x.Send(viewModel.ProviderRequestId, GasaStatus.New, requestIsFromApprovedRegion));
        }

        private bool RequestIsFromApprovedRegion(string providerData)
        {
            return !approvedRegions.Enabled || approvedRegions.Regions.Contains(providerData);
        }
    }

    public interface IProcessApiRequests
    {
        Task Process(RequestApiViewModel viewModel);
    }
}
