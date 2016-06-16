using AllReady.Models;

namespace AllReady.ViewModels
{
    public class LocationViewModel
    {
        public LocationViewModel()
        {

        }

        public LocationViewModel(Location location)
        {
            Address1 = location.Address1;
            Address2 = location.Address2;
            City = location.City;
            State = location.State;
            PostalCode = location.PostalCode;
            Country = location.Country;
        }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string Summary {
            get
            {
                if (!string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(State))
                    return $"{City}, {State}";
                if (!string.IsNullOrEmpty(City))
                    return $"{City}";
                if (!string.IsNullOrEmpty(State))
                    return $"{State}";

                return string.Empty;
            }
        }
    }
}
