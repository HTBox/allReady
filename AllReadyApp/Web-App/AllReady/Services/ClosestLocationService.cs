using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AllReady.Models;
using Microsoft.Extensions.OptionsModel;
using AllReady.Extensions;

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

        private readonly IBaseDatabaseConnection _connection;

        /// <summary>
        /// Constructor - takes in IBaseDatabaseConnection
        /// </summary>
        /// <param name="connection">The database connection</param>
        public SqlClosestLocations(IBaseDatabaseConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _connection = connection;
        }

        public IEnumerable<ClosestLocation> GetClosestLocations(LocationQuery query)
        {
            IEnumerable<ClosestLocation> ret;

            using (var connection = _connection.Get())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "GetClosestLocations";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddInputParameter("@lat", query.Latitude, DbType.Double);
                    cmd.AddInputParameter("@lon", query.Longitude, DbType.Double);
                    cmd.AddInputParameter("@count", query.MaxRecordsToReturn.HasValue ? query.MaxRecordsToReturn : 10, DbType.Int32);
                    cmd.AddInputParameter("@distance", query.Distance.HasValue ? query.Distance : 10, DbType.Int32);
                    
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    ret = cmd.ExecuteReader().DataReaderMapToList<ClosestLocation>();
                }
            }

            return ret;
        }

        public IEnumerable<PostalCodeGeoCoordinate> GetPostalCodeCoordinates(string postalCode)
        {
            IEnumerable<PostalCodeGeoCoordinate> ret;

            using (var connection = _connection.Get())
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "GetCoordinatesForPostalCode";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddInputParameter("@postalcode", postalCode, DbType.String);
                    
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();

                    ret = cmd.ExecuteReader().DataReaderMapToList<PostalCodeGeoCoordinate>();
                }
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
