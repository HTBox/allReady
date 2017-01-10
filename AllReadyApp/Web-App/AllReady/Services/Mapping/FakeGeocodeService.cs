using System;
using System.Threading.Tasks;

namespace AllReady.Services.Mapping
{
    public class FakeGeocodeService : IGeocodeService
    {
        public Task<Coordinates> GetCoordinatesFromAddress(string address)
        {
            return Task.FromResult(new Coordinates(47.624833, -122.236060));
        }

        public Task<Coordinates> GetCoordinatesFromAddress(string address, string city, string state, string postalCode, string country)
        {
            return Task.FromResult(new Coordinates(47.624833, -122.236060));
        }
    }
}
