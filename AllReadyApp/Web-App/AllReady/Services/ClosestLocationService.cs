using Microsoft.Framework.Configuration;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        private IConfiguration _config;
        private const string CONNECTION_STRING = "Data:DefaultConnection:AzureConnectionString";

        public SqlClosestLocations(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<ClosestLocation> GetClosestLocations(LocationQuery query)
        {
            IEnumerable<ClosestLocation> ret = null;

            using (var connection = new SqlConnection(_config[CONNECTION_STRING]))
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

            using (var connection = new SqlConnection(_config[CONNECTION_STRING]))
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
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }
    }
}
