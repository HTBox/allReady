using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AllReady.Models;

namespace AllReady.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToName(this UserType theType)
        {
            return Enum.GetName(typeof(UserType), theType);
        }

        /// <summary>
        /// Retrieves the Display attribute of an enum.
        /// </summary>
        /// <example>
        /// [Display(Name="Numeric Impact")]
        /// Numeric = 0
        /// ...
        /// MyEnum.GetDisplayName() // returns "Numeric Impact"
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

    }
}
