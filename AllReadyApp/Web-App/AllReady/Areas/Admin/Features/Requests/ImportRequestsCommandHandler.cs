using AllReady.Models;
using Geocoding.Google;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Requests
{
    public class ImportRequestsCommandHandler : IRequestHandler<ImportRequestsCommand, IEnumerable<ImportRequestError>>
    {
        private readonly AllReadyContext _context;

        public ImportRequestsCommandHandler(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<ImportRequestError> Handle(ImportRequestsCommand message)
        {
            var errors = new List<ImportRequestError>();
            var geocoder = new GoogleGeocoder();

            foreach (var request in message.Requests)
            {
                // todo: do basic data validation
                if (!_context.Requests.Any(r => r.ProviderId == request.ProviderId))
                {
                    //If lat/long not provided, use geocoding API to get them
                    if (request.Latitude == 0 && request.Longitude == 0)
                    {
                        //Assume the first returned address is correct
                        var address = geocoder.Geocode(string.Format("{0} {1} {2} {3}",
                            request.Address,
                            request.City,
                            request.State,
                            request.Zip
                        )).FirstOrDefault();
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
