using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AllReady.Models;

namespace AllReady.Extensions
{
    public static class EnumerableExtensions
    {

        /// <summary>
        /// Retrieves the Display attribute of an enum.
        /// </summary>
        /// <example>
        /// [Display(Name="Numeric Goal")]
        /// Numeric = 0
        /// ...
        /// MyEnum.GetDisplayName() // returns "Numeric Goal"
        /// </example>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();  // GetName (vs. Name) ensures that the localized string will be returned if using the ResourceType attribute property.
        }

        /// <summary>
        /// Returns the sequence divided into groups of the given size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The sequence to be grouped.</param>
        /// <param name="count">The size of the groups.</param>
        /// <returns>A sequence of groups.</returns>
        public static IEnumerable<IEnumerable<T>> GroupInto<T>(this IEnumerable<T> source, int count)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));

            var list = new List<T>(count);
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                    if (list.Count == count)
                    {
                        yield return list;
                        list = new List<T>(count);
                    }
                }
            }

            if (list.Count > 0)
            {
                yield return list;
            }
        }
    }
}
