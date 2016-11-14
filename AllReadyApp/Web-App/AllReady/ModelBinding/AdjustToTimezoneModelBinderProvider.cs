using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ModelBinding
{
    public class AdjustToTimezoneModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && !string.IsNullOrEmpty(context.Metadata.PropertyName))
            {
                // Look for DateTimeOffset attributes
                var propName = context.Metadata.PropertyName;
                var propInfo = context.Metadata.ContainerType.GetProperty(propName);

                // Only one scrubber attribute can be applied to each property
                var attribute = propInfo.GetCustomAttributes(typeof(AdjustToTimezoneAttribute), false).FirstOrDefault() as AdjustToTimezoneAttribute;
                if (attribute != null) return new AdjustToTimeZoneModelBinder(context.Metadata.ModelType, attribute.TimeZoneIdPropertyName);
            }

            return null;
        }
    }
}
