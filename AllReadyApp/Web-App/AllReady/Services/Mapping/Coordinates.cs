using System;
using Newtonsoft.Json;

namespace AllReady.Services.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Trimmed down version of GeoCoding.net class - https://github.com/chadly/Geocoding.net/blob/netcore/src/Geocoding.Core/Location.cs </remarks>
    public class Coordinates
    {
        private double latitude;
        private double longitude;
        
        public virtual double Latitude
        {
            get { return latitude; }
            set
            {
                if (value < -90 || value > 90)
                    throw new ArgumentOutOfRangeException("Latitude", value, "Value must be between -90 and 90 inclusive.");

                if (double.IsNaN(value))
                    throw new ArgumentException("Latitude must be a valid number.", "Latitude");

                latitude = value;
            }
        }

        public virtual double Longitude
        {
            get { return longitude; }
            set
            {
                if (value < -180 || value > 180)
                    throw new ArgumentOutOfRangeException("Longitude", value, "Value must be between -180 and 180 inclusive.");

                if (double.IsNaN(value))
                    throw new ArgumentException("Longitude must be a valid number.", "Longitude");

                longitude = value;
            }
        }

        protected Coordinates()
            : this(0, 0)
        {
        }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Coordinates(string latitude, string longitude)
        {
            double latitudeOut;
            double longitudeOut;

            if (double.TryParse(latitude, out latitudeOut))
            {
                if (double.TryParse(longitude, out longitudeOut))
                {
                    Latitude = latitudeOut;
                    Longitude = longitudeOut;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Coordinates);
        }

        public bool Equals(Coordinates coor)
        {
            if (coor == null)
                return false;

            return (this.Latitude == coor.Latitude && this.Longitude == coor.Longitude);
        }

        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Latitude.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", latitude, longitude);
        }
    }
}