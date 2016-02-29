using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllReady.Extensions
{
    public static class AttributeExtensions
    {
        public static IEnumerable<Attribute> GetAttributes<T>(this T obj)
        {
            var type = typeof(T);
            return Enumerable.Cast<Attribute>(type.GetCustomAttributes(true));
        }

        public static IEnumerable<Attribute> GetAttributesOn<T>(this T obj, Expression<Func<T, object>> expression)
        {
            IEnumerable<Attribute> attributes = new List<Attribute>();

            if (expression.Body is MethodCallExpression)
                attributes = Enumerable.Cast<Attribute>(((MethodCallExpression)expression.Body).Method.GetCustomAttributes(true));

            if (expression.Body is UnaryExpression && ((UnaryExpression)expression.Body).Operand is MemberExpression)
                attributes = Enumerable.Cast<Attribute>((((UnaryExpression)expression.Body).Operand as MemberExpression).Member.GetCustomAttributes(true));

            return attributes;
        }
    }
}