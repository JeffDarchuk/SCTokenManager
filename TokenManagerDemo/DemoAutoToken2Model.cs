using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using TokenManager.Data.TokenDataTypes.Attributes;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager
{
	public class DemoAutoToken2Model
	{
		//Indicates the user should be prompted to enter a string value when inserting this token.
		[TokenString(label: "Give us a string", placeholder: "Enter string here", required: true, defaultValue: "blorp")]public string MySecretValue { get; set; }
		public string UserName { get; set; }


		[TokenInteger("int default 4", true, 4)] public int Four { get; set; }
		[TokenInteger("int no default", true)] public int IntNoDefault { get; set; }

		[TokenBoolean("boolean no default")] public bool BoolNoDefault { get; set; }
		[TokenBoolean("boolean default", true)] public bool BoolDefault { get; set; }

		[TokenDroplist("static options no default", true, new []{"Op1", "OPTION 1", "Op2", "OPTION 2"})] public string DroplistNoDefaultStaticOps { get; set; }
		[TokenDroplist("static options default", true, new[] { "Op3", "OPTION 3", "Op4", "OPTION 4" }, "OPTION 4")] public string DroplistDefaultStaticOps { get; set; }
		[TokenDroplist("dynamic options default", true, "{62BC5816-165B-413D-B8E7-692063D12374}", true, "{8AF9B5E1-DCD3-4E8B-91DC-593AC1DBBA43}")] public string DroplistDefaultDynamicOpsIds { get; set; }
		[TokenDroplist("dynamic options default", true, "{62BC5816-165B-413D-B8E7-692063D12374}", false, "Do not index the page or follow links")] public string DroplistDefaultDynamicOps { get; set; }
		[TokenDroplist("dynamic options no default", true, "{62BC5816-165B-413D-B8E7-692063D12374}")] public string DroplistDefaultNoDynamicOpsIds { get; set; }
		[TokenDroplist("dynamic options no default", true, "{62BC5816-165B-413D-B8E7-692063D12374}", false)] public string DroplistDefaultNoDynamicOps { get; set; }

		[TokenGeneralLink("General link no default", true)] public GeneralLink NoDefaultLink { get; set; }
		[TokenGeneralLink("General link default url", true, "{62BC5816-165B-413D-B8E7-692063D12374}", "http://www.blarp.com")] public GeneralLink DefaultLinkUrl { get; set; }
		[TokenGeneralLink("General link default item", true, "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}", "{91A0D763-AE33-4B71-9D5C-497C82C7D35A}")] public GeneralLink DefaultLinkItem { get; set; }

		[TokenId("id", true)] public ID IdNoDefault;
		[TokenId("id default", true, "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}", "{91A0D763-AE33-4B71-9D5C-497C82C7D35A}")] public ID IdDefault;
	}
}
