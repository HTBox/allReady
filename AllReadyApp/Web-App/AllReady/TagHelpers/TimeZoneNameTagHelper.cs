using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
            string timeZoneDisplayName = FormatTimeZoneDisplayName(TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId).DisplayName);
            output.Content.SetContent(timeZoneDisplayName);
        }

        /// <summary>
        /// Method to format display name to remove text in parenthesis, e.g.: "(UTC-06:00) Central Time (US & Canada)" => "Central Time"
        /// </summary>
        /// <param name="unformatted">Unformatted time zone display name.</param>
        /// <returns>Time zone display text that was previously nested in parenthetical text.</returns>
        public string FormatTimeZoneDisplayName(string unformatted)
        {
            // 
            int begin = 0;
            int end = unformatted.Length;
            bool firstParen = true;

            for (int i = 0; i < end; i++)
            {
                if (firstParen && unformatted[i] == ')')
                {
                    // two char for ") "
                    begin = i + 2;
                    firstParen = false;
                }

                if (!firstParen && unformatted[i] == '(')
                {
                    end = i - 1;
                }
            }

            return unformatted.Substring(begin, end - begin);
        }
    }
}
