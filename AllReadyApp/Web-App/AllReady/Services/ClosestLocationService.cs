using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using AllReady.Models;
using Microsoft.Framework.OptionsModel;

namespace AllReady.Services
{
    public interface IClosestLocations
    {
        IEnumerable<ClosestLocation> GetClosestLocations(LocationQuery query);
        IEnumerable<PostalCodeGeoCoordinate> GetPostalCodeCoordinates(string postalCode);
    }

    public class SqlClosestLocations
        : IClosestLocations
    {
        private DatabaseSettings _settings;

        public SqlClosestLocations(IOptions<DatabaseSettings> options)
        {
            _settings = options.Value;
        }

        public IEnumerable<ClosestLocation> GetClosestLocations(LocationQuery query)
        {
            IEnumerable<ClosestLocation> ret = null;

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "GetClosestLocations";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@lat", query.Latitude);
                cmd.Parameters.AddWithValue("@lon", query.Longitude);
                cmd.Parameters.AddWithValue("@count", query.MaxRecordsToReturn.HasValue ? query.MaxRecordsToReturn : 10);
                cmd.Parameters.AddWithValue("@distance", query.Distance.HasValue ? query.Distance : 10);

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                ret = cmd.ExecuteReader().DataReaderMapToList<ClosestLocation>();
            }

            return ret;
        }

        public IEnumerable<PostalCodeGeoCoordinate> GetPostalCodeCoordinates(string postalCode)
        {
            IEnumerable<PostalCodeGeoCoordinate> ret = null;

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "GetCoordinatesForPostalCode";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@postalcode", postalCode);

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                ret = cmd.ExecuteReader().DataReaderMapToList<PostalCodeGeoCoordinate>();
            }

            return ret;
        }
    }

    public static class DataReaderExtensions
    {
        public static List<T> DataReaderMapToList<T>(this IDataReader dr)
        {
            var list = new List<T>();
            while (dr.Read())
            {
                var obj = Activator.CreateInstance<T>();
                foreach (var prop in obj
                    .GetType()
                    .GetProperties()
                    .Where(prop => !Equals(dr[prop.Name], DBNull.Value)))
                {
                    prop.SetValue(obj, dr[prop.Name], null);
                }
                list.Add(obj);
            }
            return list;
        }
    }
}
