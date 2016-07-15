using AllReady.ViewModels.Shared;

namespace AllReady.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; } = "USA";
    }

    public static class LocationExtensions
    {
        public static LocationViewModel ToViewModel(this Location location)
        {
            var value = new LocationViewModel
            {
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                PostalCode = location.PostalCode,
                State = location.State
            };
            return value;
        }
    }
}