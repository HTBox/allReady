using AllReady.Areas.Admin.ViewModels.Shared;
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
        public static LocationDisplayViewModel ToModel(this Location location)
        {
            if (location == null)
            {
                return new LocationDisplayViewModel();
            }

            return new LocationDisplayViewModel
            {
                Id = location.Id,
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                Country = location.Country,
                Name = location.Name,
                PhoneNumber = location.PhoneNumber,
                PostalCode = location.PostalCode,
                State = location.State
            };
        }

        public static LocationEditViewModel ToEditModel(this Location location)
        {
            //TODO: Do I want to keep LocationEditModel and LocationDisplayModel ??
            if (location == null)
            {
                return new LocationEditViewModel();
            }

            return new LocationEditViewModel
            {
                Id = location.Id,
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                Country = location.Country,
                Name = location.Name,
                PhoneNumber = location.PhoneNumber,
                PostalCode = location.PostalCode,
                State = location.State
            };
        }

        public static Location UpdateModel(this Location location, LocationEditViewModel locationEditModel)
        {
            if (locationEditModel != null)
            {
                var locationInfo = string.Concat(locationEditModel.Address1?.Trim(), locationEditModel.Address2?.Trim(), locationEditModel.City?.Trim(), locationEditModel.State?.Trim(), locationEditModel.PostalCode?.Trim(), locationEditModel.Name?.Trim(), locationEditModel.PhoneNumber?.Trim());
                if (string.IsNullOrWhiteSpace(locationInfo))
                {
                    location = null;
                }
                else
                {
                    if (location == null || locationEditModel.Id.GetValueOrDefault() != 0)
                    {
                        location = new Location();
                    }
                    location.Address1 = locationEditModel.Address1;
                    location.Address2 = locationEditModel.Address2;
                    location.City = locationEditModel.City;
                    location.Country = locationEditModel.Country;
                    location.Name = locationEditModel.Name;
                    location.PhoneNumber = locationEditModel.PhoneNumber;
                    location.PostalCode = locationEditModel.PostalCode;
                    location.State = locationEditModel.State;
                    return location;
                }
            }
            return location;
        }

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