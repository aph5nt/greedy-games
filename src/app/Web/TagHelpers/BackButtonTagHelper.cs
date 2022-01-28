using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Web.TagHelpers
{
    [HtmlTargetElement("backbutton", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BackButtonTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = @"
                <button class=""btn btn-default btn-xs back-button"">
                <i class=""fa fa-arrow-circle-left"" aria-hidden=""true""></i>
                </button>";

            output.Content.AppendHtml(content);
        }
    }
}