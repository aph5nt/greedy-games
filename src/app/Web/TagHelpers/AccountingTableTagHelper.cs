using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Shared.Model;

namespace Web.TagHelpers
{
    [HtmlTargetElement("tr", Attributes = TranStatusAttributeName)]
    public class AccountingTableTagHelper : TagHelper
    {
        private const string TranStatusAttributeName = "tran-status";
        private const string TranSignatureAttributeName = "tran-signature";
        private const string NetworkAttributeName = "network";

        [HtmlAttributeName(TranStatusAttributeName)]
        public TranStatus Status { get; set; } = TranStatus.Pending;

        [HtmlAttributeName(TranSignatureAttributeName)]
        public string Signature { get; set; } = string.Empty;

        [HtmlAttributeName(NetworkAttributeName)]
        public Network Network { get; set; } = Network.WAVES;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var statusClass = "";
            switch (Status)
            {
                case TranStatus.Failed:
                    statusClass =  "danger";
                    break;
                case TranStatus.Pending:
                    statusClass = "info";
                    break;
            }

            var classValue = 
                output.Attributes.ContainsName("class") ? 
                    string.Format("{0} {1}", output.Attributes["class"].Value, statusClass) :
                    statusClass;

            output.Attributes.SetAttribute("class", classValue);
            if (!String.IsNullOrEmpty(Signature))
            {
                output.Attributes.SetAttribute("data-href",
                    Network == Network.WAVES
                        ? $@"https:///wavesexplorer.com//tx//{Signature}"
                        : $@"http:///testnet.wavesexplorer.com//tx//{Signature}");
            }

            base.Process(context, output);
        }
    }
}