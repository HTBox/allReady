using AllReady.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using System.Text.Encodings.Web;

namespace AllReady.UnitTest.TagHelpers
{
    public class TimeTagHelperShould
    {
        private Func<bool, HtmlEncoder, Task<TagHelperContent>> GetEmptyChildContent()
        {
            TagHelperContent content = new DefaultTagHelperContent();
            return (b, encoder) => Task.FromResult(content);
        }

        private TagHelperContext GetContext()
        {
            return new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString());
        }

        private TagHelperOutput GetOutput()
        {
            return new TagHelperOutput("time", new TagHelperAttributeList(), GetEmptyChildContent());
        }

        [Fact]
        public void ValueShouldBeFormattedUsingDefaultFormat()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            var value = new DateTime(2014, 12, 25, 13, 21, 0);

            TimeTagHelper tagHelper = new TimeTagHelper
            {
                Value = new DateTimeOffset(value, TimeSpan.FromHours(0))
            };

            var output = GetOutput();
            tagHelper.Process(GetContext(), output);

            Assert.Null(output.TagName);
            Assert.Equal(value.ToString("g"), output.Content.GetContent());
        }

        [Fact]
        public void ValueShouldBeFormattedUsingSpecifiedFormat()
        {
            TimeTagHelper tagHelper = new TimeTagHelper
            {
                Value = new DateTimeOffset(2014, 12, 25, 13, 21, 0, TimeSpan.FromHours(0)),
                Format = "yyyy-MM-dd"
            };

            var output = GetOutput();
            tagHelper.Process(GetContext(), output);

            Assert.Null(output.TagName);
            Assert.Equal("2014-12-25", output.Content.GetContent());
        }
    }
}
