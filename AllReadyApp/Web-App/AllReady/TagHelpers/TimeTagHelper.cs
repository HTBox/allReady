using System;
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
            // this is a hack that should not be needed. Need to understand why 2.0.0 upgrade causes this to match head & body tags which causes layout exception
            // Exception = InvalidOperationException: RenderBody has not been called for the page at '/Views/Shared/_Layout.cshtml'. To ignore call IgnoreBody().
            if (output.TagName != "time") return;

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
                    TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TargetTimeZoneId);
                    var targetOffset = targetTimeZone.GetUtcOffset(dateTimeToDisplay);
                    dateTimeToDisplay = dateTimeToDisplay.ToOffset(targetOffset);
                }
                var formattedTime = dateTimeToDisplay.ToString(Format);
                output.Content.SetContent(formattedTime);
            }
        }
    }
}
