using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllReady.UnitTest.Controllers
{
    public static class AttributeExtensions
    {
        //TODO: test with something like [Key, Column(Order = 0)]
        public static IEnumerable<Attribute> GetAttributesOn<T>(this T obj, Expression<Func<T, object>> expression)
        {
            IEnumerable<Attribute> attributes = new List<Attribute>();

            if (expression.Body is MethodCallExpression)
                attributes = Enumerable.Cast<Attribute>((IEnumerable)((MethodCallExpression)expression.Body).Method.GetCustomAttributes(true));

            if (expression.Body is UnaryExpression && (((UnaryExpression)expression.Body).Operand is MemberExpression))
                attributes = Enumerable.Cast<Attribute>((IEnumerable)(((UnaryExpression)expression.Body).Operand as MemberExpression).Member.GetCustomAttributes(true));

            return attributes;
        }

        public static bool HasAttribute<T>(this T obj, Type attributeType)
        {
            var type = typeof(T);
            return type.CustomAttributes.Any(x => x.AttributeType == attributeType);
        }

        //TODO: test with something like [Key, Column(Order = 0)]
        public static bool HasAttributeWithValue<T>(this T obj, Type attributeType, string attributeValue)
        {
            var type = typeof(T);
            var areaAttribute = type.CustomAttributes.First(x => x.AttributeType == attributeType);
            var constructorArg = areaAttribute.ConstructorArguments.First().Value as string;
            return attributeValue == constructorArg;
        }

        //public static IEnumerable<Attribute> GetPropertyAttributesOn<T>(this T obj, Expression<Func<T, object>> getPropertyExpression)
        //{
        //    // ReSharper disable once PossibleNullReferenceException
        //    return Enumerable.Cast<Attribute>((IEnumerable)(!(getPropertyExpression.Body is UnaryExpression) ?
        //        (MemberExpression)getPropertyExpression.Body : 
        //        ((UnaryExpression)getPropertyExpression.Body).Operand as MemberExpression).Member.GetCustomAttributes(true));
        //}

        //public static IEnumerable<Attribute> GetMethodAttributesOn<T>(this T obj, Expression<Func<T, object>> getMethodExpression)
        //{
        //    // ReSharper disable once PossibleNullReferenceException
        //    return Enumerable.Cast<Attribute>((IEnumerable)(!(getMethodExpression.Body is UnaryExpression) ? 
        //        (MethodCallExpression)getMethodExpression.Body : 
        //        ((UnaryExpression)getMethodExpression.Body).Operand as MethodCallExpression).Method.GetCustomAttributes(true));
        //}
    }

    //original
    //public static class AttributeExtensions
    //{
    //    public static IEnumerable<Attribute> GetAttributesOn<T>(this T obj, Expression<Func<T, object>> getPropertyExpression)
    //    {
    //        return Enumerable.Cast<Attribute>((IEnumerable)(!(getPropertyExpression.Body is UnaryExpression) ? (MemberExpression)getPropertyExpression.Body : ((UnaryExpression)getPropertyExpression.Body).Operand as MemberExpression).Member.GetCustomAttributes(true));
    //    }
    //}
}