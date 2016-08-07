using System;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Geocoding;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Requests
{
    public class AddRequestCommandHandlerAsync : IAsyncRequestHandler<AddRequestCommandAsync, AddRequestError>
    {
        private readonly IAllReadyDataAccess _dataAccess;
        private readonly IGeocoder _geocoder;

        public AddRequestCommandHandlerAsync(IAllReadyDataAccess dataAccess, IGeocoder geocoder)
        {
            _dataAccess = dataAccess;
            _geocoder = geocoder;
        }

        public async Task<AddRequestError> Handle(AddRequestCommandAsync message)
        {
            AddRequestError error = null;
            if (message.Request == null)
            {
                throw new InvalidOperationException("Request property is required.");
            }

            var request = message.Request;

            try
            {
                //todo: I'm not sure if this logic is going to be correct, as this allows an update of status to existing requests.
                //I added this because the red cross is passing in current status.
                Request entity = await _dataAccess.GetRequestByProviderIdAsync(request?.ProviderId);

                if (entity == null)
                {
                    request.RequestId = Guid.NewGuid();
                    //If lat/long not provided, use geocoding API to get them
                    if (request.Latitude == 0 && request.Longitude == 0)
                    {
                        //Assume the first returned address is correct
                        var address = _geocoder.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty)
                            .FirstOrDefault();
                        request.Latitude = address?.Coordinates.Latitude ?? 0;
                        request.Longitude = address?.Coordinates.Longitude ?? 0;
                    }
                }
                else
                {
                    entity.Status = request.Status;
                }

                await _dataAccess.AddRequestAsync(entity ?? request); //Test calling with existing request
            }
            catch (Exception)
            {
                error = new AddRequestError
                {
                    ProviderId = request?.ProviderId ?? "No ProviderId.",
                    Reason = "Failed to add request."
                };

                //todo: Logging for this error
            }

            return error;
        }

    }
}
