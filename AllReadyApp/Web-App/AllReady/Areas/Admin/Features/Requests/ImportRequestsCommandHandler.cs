using System;
using AllReady.Models;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AllReady.Services.Mapping;
using AllReady.Services.Mapping.GeoCoding;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommandHandler : AsyncRequestHandler<ImportRequestsCommand>
    {
        private readonly AllReadyContext context;
        private readonly IGeocodeService geocoder;
        public Func<Guid> NewRequestId = () => Guid.NewGuid();
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public ImportRequestsCommandHandler(AllReadyContext context, IGeocodeService geocoder)
        {
            this.context = context;
            this.geocoder = geocoder;
        }

        protected override async Task HandleCore(ImportRequestsCommand message)
        {
            var orgId = await context.Events.AsNoTracking()
                .Include(rec => rec.Campaign)
                .Where(rec => rec.Id == message.EventId)
                .Select(rec => rec.Campaign.ManagingOrganizationId)
                .FirstOrDefaultAsync();

            if (orgId > 0)
            {
                var requests = message.ImportRequestViewModels.Select(viewModel => new Request
                {
                    OrganizationId = orgId,
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
                    PostalCode = viewModel.PostalCode,
                    Status = RequestStatus.Unassigned,
                    Source = RequestSource.Csv
                }).ToList();

                context.Requests.AddRange(requests);

                //TODO mgmccarthy: eventually move IGeocoder invocations to async using azure. Issue #1626 and #1639
                foreach (var request in requests)
                {
                    if (request.Latitude == 0 && request.Longitude == 0)
                    {
                        var coordinates = await geocoder.GetCoordinatesFromAddress(request.Address, request.City, request.State, request.PostalCode, string.Empty);

                        request.Latitude = coordinates?.Latitude ?? 0;
                        request.Longitude = coordinates?.Longitude ?? 0;
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}