using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Geocoding;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Requests
{
    public class AddRequestCommandHandler : AsyncRequestHandler<AddRequestCommand>
    {
        private readonly AllReadyContext _context;
        private readonly IGeocoder _geocoder;
        public Func<Guid> RequestId = () => Guid.NewGuid();

        public AddRequestCommandHandler(AllReadyContext context, IGeocoder geocoder)
        {
            _context = context;
            _geocoder = geocoder;
        }

        protected override async Task HandleCore(AddRequestCommand message)
        {
            var request = message.Request;

            //todo: I'm not sure if this logic is going to be correct, as this allows an update of status to existing requests.
            //I added this because the red cross is passing in current status.
            //mgmccarthy: so I'm confused... we're querying here for a request that matches the provider id of the incoming message, and then using that to gauge whether or not
            //this request already exists? If that's the case, then this code is wrong
            var existingRequest = await _context.Requests.FirstOrDefaultAsync(x => x.ProviderId == request.ProviderId);

            //mgmccarthy: so my thinking here is this command handler handles not only requests that are new, but requests that exist in the db and that we need to update?
            //when would we ever get a status update from an existing request WE created from an outside source?
            if (existingRequest == null)
            {
                //public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;
                //request.RequestId = Guid.NewGuid();
                //If lat/long not provided, use geocoding API to get them
                if (request.Latitude == 0 && request.Longitude == 0)
                {
                    //Assume the first returned address is correct
                    var address = _geocoder.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty)
                        .FirstOrDefault();
                    request.Latitude = address?.Coordinates.Latitude ?? 0;
                    request.Longitude = address?.Coordinates.Longitude ?? 0;
                }
                existingRequest = request;
                _context.Requests.Add(existingRequest);
            }
            else
            {
                //mgmccarthy: instead of doing this work here, we should send RequestStatusChangeCommand which will update the status of a request for us given a RequestId and RequestStatus
                existingRequest.Status = request.Status;
            }

            //await context.AddRequestAsync(entity ?? request);
            //_dbContext.Requests.Add(request);
            //await _dbContext.SaveChangesAsync();

            _context.Requests.Add(existingRequest ?? request);

            await _context.SaveChangesAsync();
        }
    }
}