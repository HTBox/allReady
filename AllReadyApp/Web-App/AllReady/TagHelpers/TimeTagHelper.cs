using Microsoft.AspNet.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.TagHelpers
{
    [HtmlTargetElement("time", Attributes="value")]
    public class TimeTagHelper : TagHelper
    {
        [HtmlAttributeName("value")]
        public DateTimeOffset? Value { get; set; }

        [HtmlAttributeName("format")]
        public string Format { get; set; } = "g";

        [HtmlAttributeName("target-time-zone-id")]
        public string TargetTimeZoneId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            
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
