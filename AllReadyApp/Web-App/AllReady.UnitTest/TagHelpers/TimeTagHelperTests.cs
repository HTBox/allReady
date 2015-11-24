using AllReady.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.TagHelpers
{
    public class TimeTagHelperTests
    {
        private Func<bool, Task<TagHelperContent>> GetEmptyChildContent()
        {
            TagHelperContent content = new DefaultTagHelperContent();
            return b => Task.FromResult(content);
        }

        private TagHelperContext GetContext()
        {
            return new TagHelperContext(new IReadOnlyTagHelperAttribute[] { }, new Dictionary<object, object>(), Guid.NewGuid().ToString());
        }

        private TagHelperOutput GetOutput()
        {
            return new TagHelperOutput("time", new TagHelperAttributeList(), GetEmptyChildContent());
        }

        [Fact]
        public void ValueShouldBeFormattedUsingDefaultFormat()
        {
            TimeTagHelper tagHelper = new TimeTagHelper();
            tagHelper.Value = new DateTimeOffset(2014, 12, 25, 13, 21, 0, TimeSpan.FromHours(0));

            var output = GetOutput();
            tagHelper.Process(GetContext(), output);

            Assert.Null(output.TagName);
            Assert.Equal("12/25/2014 1:21 PM", output.Content.GetContent());
        }

        [Fact]
        public void ValueShouldBeFormattedUsingSpecifiedFormat()
        {
            TimeTagHelper tagHelper = new TimeTagHelper();
            tagHelper.Value = new DateTimeOffset(2014, 12, 25, 13, 21, 0, TimeSpan.FromHours(0));
            tagHelper.Format = "yyyy-MM-dd";

            var output = GetOutput();
            tagHelper.Process(GetContext(), output);

            Assert.Null(output.TagName);
            Assert.Equal("2014-12-25", output.Content.GetContent());
        }

        [Fact]
        public void ValueShouldBeConvertedToSpecifiedTimeZone()
        {
            TimeTagHelper tagHelper = new TimeTagHelper();
            tagHelper.Value = new DateTimeOffset(2014, 12, 25, 13, 21, 0, TimeSpan.FromHours(0));
            tagHelper.Format = "yyyy-MM-dd h:mm tt";
            tagHelper.TargetTimeZoneId = "Central Standard Time";

            var output = GetOutput();
            tagHelper.Process(GetContext(), output);

            Assert.Null(output.TagName);
            Assert.Equal("2014-12-25 7:21 AM", output.Content.GetContent());

        }
    }
}
