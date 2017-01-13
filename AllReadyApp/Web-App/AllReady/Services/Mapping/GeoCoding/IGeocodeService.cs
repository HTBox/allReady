using System.Threading.Tasks;
using AllReady.Services.Mapping.GeoCoding.Models;

namespace AllReady.Services.Mapping.GeoCoding
{
    /// <summary>
    /// A service which provides geocoding features
    /// </summary>
    public interface IGeocodeService
    {
        /// <summary>
        /// Get <see cref="Coordinates"/> for a given address string
        /// </summary>
        Task<Coordinates> GetCoordinatesFromAddress(string address);

        /// <summary>
        /// Get <see cref="Coordinates"/> for the component parts of an address
        /// </summary>
        Task<Coordinates> GetCoordinatesFromAddress(string address, string city, string state, string postalCode, string country);
    }
}
