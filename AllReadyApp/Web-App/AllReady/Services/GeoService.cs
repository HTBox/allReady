using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Services
{
    /*
    public partial class GeoService
    {
        static Dictionary<string, PostalCodeGeo> _postalCodes = new Dictionary<string, PostalCodeGeo>();

        static GeoService()
        {
            var lines = GetRawData().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                string[] fields = line.Split(',');

                
                Decimal latitude;
                Decimal longitude;
                if (fields.Length < 5 ||
                    !Decimal.TryParse(fields[3], NumberStyles.Number, CultureInfo.InvariantCulture, out latitude) ||
                    !Decimal.TryParse(fields[4], NumberStyles.Number, CultureInfo.InvariantCulture, out longitude))
                {
                    Debug.WriteLine("Bad geo code data: " + line);
                    continue;
                }

                string postalCode = fields[2];

                PostalCodeGeo postalCodeGeo = new PostalCodeGeo();
                postalCodeGeo.PostalCode = postalCode;
                postalCodeGeo.Latitude = latitude;
                postalCodeGeo.Longitude = longitude;

                _postalCodes[postalCode] = postalCodeGeo;
            }
        }

        static double ToRadians(double degrees)
        {
            return Math.PI * degrees / 180.0;
        }

        public double GetDistance(PostalCodeGeo geo1, PostalCodeGeo geo2)
        {
            double radiusInMeters = 6371000;
            var latRad1 = ToRadians((double)geo1.Latitude);
            var latRad2 = ToRadians((double)geo2.Latitude);
            var latDiff = ToRadians((double)(geo2.Latitude - geo1.Latitude));
            var lonDiff = ToRadians((double)(geo2.Longitude - geo1.Longitude));

            var a = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2) +
                    Math.Cos(latRad1) * Math.Cos(latRad2) *
                    Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1- a));

            return radiusInMeters * c;
        }

        public double GetDistance(string pc1, string pc2)
        {
            PostalCodeGeo geo1;
            PostalCodeGeo geo2;

            if (!_postalCodes.TryGetValue(pc1, out geo1) ||
                !_postalCodes.TryGetValue(pc2, out geo2))
            {
                return double.NaN;
            }

            return GetDistance(geo1, geo2);
        }

        public PostalCodeGeo LookupPostalCode(string postalCode)
        {
            PostalCodeGeo ret = null;
            _postalCodes.TryGetValue(postalCode, out ret);
            return ret;
        }        
    }
    */
}
