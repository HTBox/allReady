using System.Collections.Generic;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        public IEnumerable<ClosestLocation> GetClosestLocations(LocationQuery query)
        {
            return _dbContext.ClosestLocations.FromSql(
                "EXEC GetClosestLocations {0}, {1}, {2}, {3}", 
                query.Latitude, 
                query.Longitude, 
                query.MaxRecordsToReturn.HasValue ? query.MaxRecordsToReturn : 10, 
                query.Distance.HasValue ? query.Distance : 10
            );
        }

        public IEnumerable<PostalCodeGeoCoordinate> GetPostalCodeCoordinates(string postalCode)
        {
            return _dbContext.PostalCodeGeoCoordinates.FromSql(
                "EXEC GetCoordinatesForPostalCode '{0}'",
                postalCode
            );
        }
    }
}
