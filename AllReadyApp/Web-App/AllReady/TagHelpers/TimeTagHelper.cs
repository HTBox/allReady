using System;
using TimeZoneConverter;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AllReady.TagHelpers
{
    /// <summary>
    /// Formats a DateTimeOffset
    /// </summary>
    [HtmlTargetElement("time", Attributes="value")]
    public class TimeTagHelper : TagHelper
    {
        /// <summary>
        /// The DateTimeOffset value to format
        /// </summary>
        [HtmlAttributeName("value")]
        public DateTimeOffset? Value { get; set; }

        /// <summary>
        /// The DateTime format string used to format the value.
        /// Default of "g" is used if none is specified
        /// </summary>
        [HtmlAttributeName("format")]
        public string Format { get; set; } = "g";

        /// <summary>
        /// If specified, the value will be converted to this target time zone
        /// </summary>
        [HtmlAttributeName("target-time-zone-id")]
        public string TargetTimeZoneId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            if (!Value.HasValue)
            {
                output.Content.SetContent("*");
            }
            else
            {
                var dateTimeToDisplay = Value.Value;
                if (!string.IsNullOrEmpty(TargetTimeZoneId))
                {
                    TimeZoneInfo targetTimeZone = TZConvert.GetTimeZoneInfo(TargetTimeZoneId);
                    var targetOffset = targetTimeZone.GetUtcOffset(dateTimeToDisplay);
                    dateTimeToDisplay = dateTimeToDisplay.ToOffset(targetOffset);
                }
                var formattedTime = dateTimeToDisplay.ToString(Format);
                output.Content.SetContent(formattedTime);
            }
        }
    }
}
