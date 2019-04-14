using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllReady.TagHelpers
{
    [HtmlTargetElement(Attributes = "is-active-route")]
    public class ActiveRouteTagHelper : TagHelper
    {
        private IDictionary<string, string> _routeValues;

        /// <summary>
        /// Name of the action method
        /// </summary>
        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        /// <summary>
        /// Name of the controller
        /// </summary>
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        /// <summary>
        /// Name of the razor page (e.g. /index)
        /// </summary>
        [HtmlAttributeName("asp-page")]
        public string Page { get; set; }

        /// <summary>
        /// Additional route parameters
        /// </summary>
        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
        public IDictionary<string, string> RouteValues
        {
            get
            {
                if (_routeValues == null)
                    _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return _routeValues;
            }
            set
            {
                _routeValues = value;
            }
        }

        /// <summary>
        /// The ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ShouldBeActive())
            {
                MakeActive(output);
            }

            output.Attributes.RemoveAll("is-active-route");
        }

        private bool ShouldBeActive()
        {
            if (ViewContext.RouteData.Values.ContainsKey("Controller"))
            {
                return ShouldBeActieMvc();
            }
            else if (ViewContext.RouteData.Values.ContainsKey("page"))
            {
                return ShouldBeActiveRazorPage();
            }
            return true;
        }

        private bool ShouldBeActieMvc()
        {
            if (string.IsNullOrWhiteSpace(Controller) &&
                string.IsNullOrWhiteSpace(Action) &&
                !RouteValues.Any(r => !ViewContext.RouteData.Values.ContainsKey(r.Key)))
            {
                return false;
            }

            string currentController = ViewContext.RouteData.Values["Controller"].ToString();
            string currentAction = ViewContext.RouteData.Values["Action"].ToString();

            if (!string.IsNullOrWhiteSpace(Controller) && Controller.ToLower() != currentController.ToLower())
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Action) && Action.ToLower() != currentAction.ToLower())
            {
                return false;
            }

            foreach (var routeValue in RouteValues)
            {
                if (!ViewContext.RouteData.Values.ContainsKey(routeValue.Key) ||
                    ViewContext.RouteData.Values[routeValue.Key].ToString() != routeValue.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ShouldBeActiveRazorPage()
        {
            string currentPage = ViewContext.RouteData.Values["Page"].ToString();

            if (string.IsNullOrWhiteSpace(Page) || Page.ToLower() != currentPage.ToLower())
            {
                return false;
            }
            return true;
        }

        private void MakeActive(TagHelperOutput output)
        {
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");
            if (classAttr == null)
            {
                classAttr = new TagHelperAttribute("class", "active");
                output.Attributes.Add(classAttr);
            }
            else if (classAttr.Value == null || classAttr.Value.ToString().IndexOf("active") < 0)
            {
                output.Attributes.SetAttribute("class", classAttr.Value == null
                    ? "active"
                    : classAttr.Value.ToString() + " active");
            }
        }
    }
}
