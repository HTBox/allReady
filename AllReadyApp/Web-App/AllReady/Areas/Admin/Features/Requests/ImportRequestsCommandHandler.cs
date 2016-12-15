using System;
using AllReady.Models;
using Geocoding;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommandHandler : RequestHandler<ImportRequestsCommand>
    {
        private readonly AllReadyContext context;
        private readonly IGeocoder geocoder;
        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public ImportRequestsCommandHandler(AllReadyContext context, IGeocoder geocoder)
        {
            this.context = context;
            this.geocoder = geocoder;
        }

        protected override void HandleCore(ImportRequestsCommand message)
        {
            var requests = message.ImportRequestViewModels.Select(viewModel => new Request
            {
                RequestId = NewRequestId(),
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
                Source = RequestSource.Csv
            });
            
            context.Requests.AddRange(requests);
            context.SaveChanges();

            //TODO: meh, having to call geocoder here for each incoming request is going to be expensive, and could be seen as a denial of service attack again google maps. go async on this one using IAsyncGeocoder and/or use azure storage/Hangfire?
            //If lat/long not provided, use geocoding API to get them
            //        if (request.Latitude == 0 && request.Longitude == 0)
            //        {
            //            //Assume the first returned address is correct
            //            var address = _geocoder.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty)
            //                .FirstOrDefault();
            //            request.Latitude = address?.Coordinates.Latitude ?? 0;
            //            request.Longitude = address?.Coordinates.Longitude ?? 0;
            //        }
        }
    }
}