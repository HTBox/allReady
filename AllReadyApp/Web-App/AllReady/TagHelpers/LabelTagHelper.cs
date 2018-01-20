using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AllReady.TagHelpers
{
    [HtmlTargetElement("label", Attributes=ForAttributeName)]  
    public class LabelRequiredTagHelper: LabelTagHelper  
    {  
        private const string ForAttributeName = "asp-for";  
   
        public LabelRequiredTagHelper(IHtmlGenerator generator) : base(generator)  
        {  
        }  
   
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)  
        {  
            await base.ProcessAsync(context, output);  

            // Automatically mark required fields with an asterisk, except booleans
            // because MVC always sets IsRequired to true for booleans.
            if (For.Metadata.IsRequired && For.Metadata.ModelType.FullName != "System.Boolean")  
            {  
                var span = new TagBuilder("span");  
                span.AddCssClass("required");
                span.InnerHtml.Append("*");
                output.Content.AppendHtml(span);  
            }  
        }  
    }  
}
