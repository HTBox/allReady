using AllReady.Models;

namespace AllReady.Areas.Admin.Models
{
    public static class LocationExtensions
    {
        public static LocationDisplayModel ToModel(this Location location)
        {
            if (location == null)
            {
                return new LocationDisplayModel();
            }

            return new LocationDisplayModel
            {
                Id = location.Id,
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                Country = location.Country,
                Name = location.Name,
                PhoneNumber = location.PhoneNumber,
                PostalCode = location.PostalCode?.PostalCode,
                State = location.State
            };
        }

        public static LocationEditModel ToEditModel(this Location location)
        {
            //TODO: Do I want to keep LocationEditModel and LocationDisplayModel ??
            if (location == null)
            {
                return new LocationEditModel();
            }

            return new LocationEditModel
            {
                Id = location.Id,
                Address1 = location.Address1,
                Address2 = location.Address2,
                City = location.City,
                Country = location.Country,
                Name = location.Name,
                PhoneNumber = location.PhoneNumber,
                PostalCode = location.PostalCode?.PostalCode,
                State = location.State
            };            
        }

        public static Location UpdateModel(this Location location, LocationEditModel locationEditModel)
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
                    if (!string.IsNullOrWhiteSpace(locationEditModel.PostalCode))
                    {
                        location.PostalCode = new PostalCodeGeo { PostalCode = locationEditModel.PostalCode, City = locationEditModel.City, State = locationEditModel.State };
                    }
                    location.State = locationEditModel.State;
                    return location;
                }
            }
            return location;
        }
    }
}