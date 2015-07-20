using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrepOps.Models
{
    public class ClosestLocation
    {
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Distance { get; set; }
    }

    public class LocationQuery
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<int> Distance { get; set; }
        public Nullable<int> MaxRecordsToReturn { get; set; }
    }

    public class PostalCodeGeoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
