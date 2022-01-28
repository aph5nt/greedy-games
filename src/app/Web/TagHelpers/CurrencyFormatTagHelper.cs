using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Threading;
using Shared.Model;

namespace Web.TagHelpers
{
    [HtmlTargetElement("span", Attributes = AmountAttributeName)]
    public class CurrencyFormatTagHelper : TagHelper
    {
        private const string AmountAttributeName = "amount";
        private const string NetworkAttributeName = "network";

        [HtmlAttributeName(AmountAttributeName)]
        public long AmountValue { get; set; } = 0L;

        [HtmlAttributeName(NetworkAttributeName)]
        public Network Network { get; set; } = Network.FREE;

        public CurrencyFormatTagHelper()
        {
            var customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var result = (decimal)AmountValue / Money.Sathoshi;
            output.Content.AppendHtml($"{result:F8} {Network.ToString().ToUpper()}");
            base.Process(context, output);
        }
    }
}
