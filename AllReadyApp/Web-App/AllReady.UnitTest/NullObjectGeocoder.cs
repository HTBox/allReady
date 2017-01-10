using AllReady.Services.Mapping;
using System.Threading.Tasks;

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
