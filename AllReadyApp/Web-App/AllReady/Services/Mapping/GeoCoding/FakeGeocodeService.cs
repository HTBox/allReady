using System.Threading.Tasks;
using AllReady.Services.Mapping.GeoCoding.Models;

namespace AllReady.Services.Mapping.GeoCoding
{
    /// <summary>
    /// A development GeoCode Service which does not require API keys / network connectivity 
    /// </summary>
    public class FakeGeocodeService : IGeocodeService
    {
        /// <inheritdoc />
        public Task<Coordinates> GetCoordinatesFromAddress(string address)
        {
            return Task.FromResult(new Coordinates(47.624833, -122.236060));
        }

        /// <inheritdoc />
        public Task<Coordinates> GetCoordinatesFromAddress(string address, string city, string state, string postalCode, string country)
        {
            return Task.FromResult(new Coordinates(47.624833, -122.236060));
        }
    }
}
