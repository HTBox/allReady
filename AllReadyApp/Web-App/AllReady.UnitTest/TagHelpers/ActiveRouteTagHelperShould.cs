using AllReady.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.TagHelpers
{
    public class ActiveRouteTagHelperShould
    {
        private const string controllerName = "Home";
        private const string actionName = "List";
        private Dictionary<string, string> routeValue = new Dictionary<string, string> { { "Id", "2" } };
        private const string page = "/index";

        private const string notMatchingControllerName = "HomeX";
        private const string notMatchingActionName = "Index";
        private Dictionary<string, string> notMathingRouteValue = new Dictionary<string, string> { { "Id", "3" } };
        private const string notMatchingPage = "/index2";

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

        private ViewContext GetViewContextMVC()
        {
            var viewContext = new ViewContext();
            viewContext.RouteData = new Microsoft.AspNetCore.Routing.RouteData();
            viewContext.RouteData.Values.Add("Controller", controllerName);
            viewContext.RouteData.Values.Add("Action", actionName);
            viewContext.RouteData.Values.Add("Id", "2");
            return viewContext;
        }

        private ViewContext GetViewContextPage()
        {
            var viewContext = new ViewContext();
            viewContext.RouteData = new Microsoft.AspNetCore.Routing.RouteData();
            viewContext.RouteData.Values.Add("page", page);
            return viewContext;
        }

        [Fact]
        public void NotHaveClassAttributeIfNoClassPassedInAndIfNoParametersPassedIn()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextMVC()
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(0, output.Attributes.Count);
        }

        [Fact]
        public void HaveActiveClassAttributeWhenControllerMatch()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextMVC(),
                Controller = controllerName
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(1, output.Attributes.Count);
            Assert.Equal("class", output.Attributes[0].Name);
            Assert.Equal("active", output.Attributes[0].Value);
        }

        [Fact]
        public void NotHaveClassAttributeWhenControllerNotMatching()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextMVC(),
                Controller = notMatchingControllerName
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(0, output.Attributes.Count);
        }

        [Fact]
        public void HaveActiveClassAttributeWhenActionMatch()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextMVC(),
                Controller = controllerName,
                Action = actionName
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(1, output.Attributes.Count);
            Assert.Equal("class", output.Attributes[0].Name);
            Assert.Equal("active", output.Attributes[0].Value);
        }

        [Fact]
        public void NotHaveClassAttributeWhenActionNotMatching()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextMVC(),
                Controller = controllerName,
                Action = notMatchingActionName
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(0, output.Attributes.Count);
        }

        [Fact]
        public void HaveActiveClassAttributeWhenRouteDataMatch()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextMVC(),
                Controller = controllerName,
                Action = actionName,
                RouteValues = routeValue
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(1, output.Attributes.Count);
            Assert.Equal("class", output.Attributes[0].Name);
            Assert.Equal("active", output.Attributes[0].Value);
        }

        [Fact]
        public void NotHaveClassAttributeWhenRouteDataNotMatching()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextMVC(),
                Controller = controllerName,
                Action = actionName,
                RouteValues = notMathingRouteValue
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(0, output.Attributes.Count);
        }

        [Fact]
        public void HaveActiveClassAttributePageMatch()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextPage(),
                Page = page
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(1, output.Attributes.Count);
            Assert.Equal("class", output.Attributes[0].Name);
            Assert.Equal("active", output.Attributes[0].Value);
        }

        [Fact]
        public void NotHaveClassAttributeWhenPageNotMatching()
        {
            ActiveRouteTagHelper tagHelper = new ActiveRouteTagHelper
            {
                ViewContext = GetViewContextPage(),
                Page = notMatchingPage
            };
            var output = GetOutput();
            tagHelper.Process(GetContext(), output);
            Assert.Equal(0, output.Attributes.Count);
        }
    }
}
