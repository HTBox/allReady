using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AllReady.UnitTest.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsOrderedByAscending<T, TProperty>(this List<T> list, Expression<Func<T, TProperty>> propertyExpression) where TProperty : IComparable<TProperty>
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
            IComparable<TProperty> comparable = null;
            for (var index = 0; index < Enumerable.Count(list); ++index)
            {
                var other = (TProperty)propertyInfo.GetValue(list[index], null);
                if (comparable == null)
                {
                    comparable = other;
                }
                else
                {
                    if (comparable.CompareTo(other) > 0)
                        return false;
                    comparable = other;
                }
            }
            return true;
        }
    }
}
