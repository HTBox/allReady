using AllReady.Models;
using Geocoding;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommandHandler : IRequestHandler<ImportRequestsCommand, IEnumerable<ImportRequestError>>
    {
        private readonly AllReadyContext _context;
        private readonly IGeocoder _geocoder;

        public ImportRequestsCommandHandler(AllReadyContext context, IGeocoder geocoder)
        {
            _context = context;
            _geocoder = geocoder;
        }

        public IEnumerable<ImportRequestError> Handle(ImportRequestsCommand message)
        {
            var errors = new List<ImportRequestError>();

            foreach (var request in message.Requests)
            {
                request.Source = RequestSource.Csv;

                // todo: do basic data validation
                if (!_context.Requests.Any(r => r.ProviderId == request.ProviderId))
                {
                    //If lat/long not provided, use geocoding API to get them
                    if (request.Latitude == 0 && request.Longitude == 0)
                    {
                        //Assume the first returned address is correct
                        var address = _geocoder.Geocode(request.Address, request.City, request.State, request.Zip, string.Empty)
                            .FirstOrDefault();
                        request.Latitude = address?.Coordinates.Latitude ?? 0;
                        request.Longitude = address?.Coordinates.Longitude ?? 0;
                    }

                    _context.Requests.Add(request);
                }
            }
            _context.SaveChanges();

            return errors;
        }

    }
}
