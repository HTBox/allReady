using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Extensions
{

    /// <summary>
    /// Extensions for IDbCommand
    /// </summary>
    public static class DbCommandExtensions
    {

        /// <summary>
        /// "AddWithValue" for IDbCommand
        /// </summary>
        /// <typeparam name="T">The parameter value</typeparam>
        /// <param name="cmd">The IDbCommand</param>
        /// <param name="paramName">The parameter name (include @).</param>
        /// <param name="paramValue">The parameter value</param>
        /// <param name="type">The DbType</param>
        /// <returns>Position</returns>
        public static int AddInputParameter<T>(this IDbCommand cmd, string paramName, T paramValue, DbType type)
        {
            IDbDataParameter param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;

            return cmd.Parameters.Add(param);
        }

    }
}
