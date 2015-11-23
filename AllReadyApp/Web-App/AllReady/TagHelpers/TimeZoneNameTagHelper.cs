using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Razor.TagHelpers;

namespace AllReady.TagHelpers
{    
    [HtmlTargetElement("time-zone-name", Attributes="time-zone-id")]
    public class TimeZoneNameTagHelper : TagHelper
    {
        [HtmlAttributeName("time-zone-id")]
        public string TimeZoneId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Content.SetContent(TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId).DisplayName);
        }
    }
}
