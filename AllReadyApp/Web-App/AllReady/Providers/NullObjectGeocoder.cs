using System.Collections.Generic;
using Geocoding;

namespace AllReady.Providers
{
    public class NullObjectGeocoder : IGeocoder
    {
        public IEnumerable<Address> Geocode(string address)
        {
            return new List<Address>();
        }

        public IEnumerable<Address> Geocode(string street, string city, string state, string postalCode, string country)
        {
            return new List<Address>();
        }

        public IEnumerable<Address> ReverseGeocode(Location location)
        {
            return new List<Address>();
        }

        public IEnumerable<Address> ReverseGeocode(double latitude, double longitude)
        {
            return new List<Address>();
        }
    }
}