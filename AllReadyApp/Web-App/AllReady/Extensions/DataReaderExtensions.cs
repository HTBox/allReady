using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Extensions
{
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
