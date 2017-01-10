using System.Threading.Tasks;

namespace AllReady.Services.Mapping
{
    public interface IGeocodeService
    {
        Task<Coordinates> GetCoordinatesFromAddress(string address);
        Task<Coordinates> GetCoordinatesFromAddress(string address, string city, string state, string postalCode, string country);
    }
}
