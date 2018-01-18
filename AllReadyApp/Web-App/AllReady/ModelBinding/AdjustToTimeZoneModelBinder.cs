using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace AllReady.ModelBinding
{
    public class AdjustToTimeZoneModelBinder : IModelBinder
    {
        //IScrubberAttribute _attribute;
        SimpleTypeModelBinder _baseBinder;
        private string _timeZoneIdPropertyName;

        public AdjustToTimeZoneModelBinder(Type type, string timeZoneIdPropertyName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            //_attribute = attribute as IScrubberAttribute;
            _baseBinder = new SimpleTypeModelBinder(type);
            _timeZoneIdPropertyName = timeZoneIdPropertyName;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            _baseBinder.BindModelAsync(bindingContext);


            if (bindingContext.Result.IsModelSet)
            {
                var timeZoneId = bindingContext.ValueProvider.GetValue(_timeZoneIdPropertyName).FirstValue;
                var timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
                DateTimeOffset dateTimeOffsetModel = (DateTimeOffset)bindingContext.Result.Model;
                var adjustedDateTimeOffset = new DateTimeOffset(dateTimeOffsetModel.DateTime, timeZone.GetUtcOffset(dateTimeOffsetModel.DateTime));
                bindingContext.Result = ModelBindingResult.Success(adjustedDateTimeOffset);
            }

            return Task.CompletedTask;
        }
    }
}
