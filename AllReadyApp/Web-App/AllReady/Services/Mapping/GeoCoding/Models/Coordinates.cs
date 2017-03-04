using System;
using System.Globalization;

namespace AllReady.Services.Mapping.GeoCoding.Models
{
    /// <summary>
    /// Represents world position coordinates - latitude and longitude
    /// </summary>
    /// <remarks>Trimmed down version of GeoCoding.net class - https://github.com/chadly/Geocoding.net/blob/netcore/src/Geocoding.Core/Location.cs </remarks>
    public class Coordinates
    {
        private double _latitude;
        private double _longitude;
        
        /// <summary>
        /// The latitude of the coordinate pair
        /// </summary>
        public virtual double Latitude
        {
            get { return _latitude; }
            set
            {
                if (value < -90 || value > 90)
                    throw new ArgumentOutOfRangeException("Latitude", value, "Value must be between -90 and 90 inclusive.");

                if (double.IsNaN(value))
                    throw new ArgumentException("Latitude must be a valid number.", "Latitude");

                _latitude = value;
            }
        }

        /// <summary>
        /// The longitude of the coordinate pair
        /// </summary>
        public virtual double Longitude
        {
            get { return _longitude; }
            set
            {
                if (value < -180 || value > 180)
                    throw new ArgumentOutOfRangeException("Longitude", value, "Value must be between -180 and 180 inclusive.");

                if (double.IsNaN(value))
                    throw new ArgumentException("Longitude must be a valid number.", "Longitude");

                _longitude = value;
            }
        }

        protected Coordinates()
            : this(0, 0)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Coordinates"/> object from the lat and long values
        /// </summary>
        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new <see cref="Coordinates"/> object from the lat and long values
        /// </summary>
        public Coordinates(string latitude, string longitude)
        {
            double latitudeOut;
            if (!double.TryParse(latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out latitudeOut)) return;

            double longitudeOut;
            if (!double.TryParse(longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out longitudeOut)) return;

            Latitude = latitudeOut;
            Longitude = longitudeOut;
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
            return string.Format("{0}, {1}", _latitude, _longitude);
        }
    }
}