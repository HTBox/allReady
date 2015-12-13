using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Services
{

    /// <summary>
    /// Interface for Database Connection
    /// </summary>
    public interface IBaseDatabaseConnection
    {

        /// <summary>
        /// Get a connection to the database
        /// </summary>
        /// <returns>IDbConnection</returns>
        IDbConnection Get();
    }

    /// <summary>
    /// Get a Database connection
    /// </summary>
    public class BaseDatabaseConnection : IBaseDatabaseConnection
    {

        #region Private
        private readonly DatabaseSettings _settings;
        #endregion Private

        #region Constructor

        /// <summary>
        /// Constructor - takes in connection string for database connection
        /// </summary>
        /// <param name="options">Database settings</param>
        public BaseDatabaseConnection(IOptions<DatabaseSettings> options)
        {
            if (options == null)
                throw new ArgumentException(nameof(options));

            this._settings = options.Value;
        }

        #endregion Constructor

        #region Public methods

        /// <summary>
        /// Get a connection to the database
        /// </summary>
        /// <param name="connectionString">The connection string to use</param>
        /// <returns>IDbConnection</returns>
        public IDbConnection Get()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _settings.ConnectionString;

            return connection;
        }

        #endregion

    }
}
