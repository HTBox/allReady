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
                ProviderRequestId = viewModel.Id,
                EventId = message.EventId,
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
            }).ToList();
            
            context.Requests.AddRange(requests);

            //TODO mgmccarthy: eventually move IGeocoder invocations to async using azure. Issue #1626 and #1639
            foreach (var request in requests)
            {
                if (request.Latitude == 0 && request.Longitude == 0)
                {
                    //Assume the first returned address is correct
                    var address = geocoder.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty).FirstOrDefault();
                    request.Latitude = address?.Coordinates.Latitude ?? 0;
                    request.Longitude = address?.Coordinates.Longitude ?? 0;
                }
            }

            context.SaveChanges();
        }
    }
}