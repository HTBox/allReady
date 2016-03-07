using System;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace AllReady.UnitTest.Extensions
{
    public static class JsonResultExtensions
    {
        public static T GetValueForProperty<T>(this JsonResult jsonResult, string propertyName)
        {
            var property = jsonResult.Value.GetType().GetProperties().FirstOrDefault(p => string.CompareOrdinal(p.Name, propertyName) == 0);
            if (null == property)
                throw new ArgumentException($"propertyName: {propertyName} not found");

            return (T)property.GetValue(jsonResult.Value, null);
        }
    }
}
