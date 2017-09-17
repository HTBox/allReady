using System.Threading.Tasks;
using AllReady.Services.Mapping.GeoCoding;
using AllReady.Services.Mapping.GeoCoding.Models;

namespace AllReady.UnitTest
{
    public class NullObjectGeocoder : IGeocodeService
    {
        public Task<Coordinates> GetCoordinatesFromAddress(string address)
        {
            return Task.FromResult(new Coordinates(0, 0));
        }

        public Task<Coordinates> GetCoordinatesFromAddress(string address, string city, string state, string postalCode, string country)
        {
            return Task.FromResult(new Coordinates(0, 0));
        }
    }
}
