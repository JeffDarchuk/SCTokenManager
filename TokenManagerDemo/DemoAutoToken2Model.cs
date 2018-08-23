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
		public string UserName { get; set; }


		//Indicates the user should be prompted to enter a string value when inserting this token.
		[TokenString(label: "Give us a string", placeholder: "Enter string here", required: true, defaultValue: "blorp")]
		public string MySecretValue { get; set; }

		[TokenInteger("int default 4", true, 4)]
		public int Four { get; set; }
		[TokenInteger("int no default", true)]
		public int IntNoDefault { get; set; }

		[TokenBoolean("boolean no default")]
		public bool BoolNoDefault { get; set; }
		[TokenBoolean("boolean default", true)]
		public bool BoolDefault { get; set; }

		[TokenDroplist("static options no default", true, new []{"Op1", "OPTION 1", "Op2", "OPTION 2"})]
		public string DroplistNoDefaultStaticOps { get; set; }
		[TokenDroplist("static options default", true, new[] { "Op3", "OPTION 3", "Op4", "OPTION 4" }, "OPTION 4")]
		public string DroplistDefaultStaticOps { get; set; }
		[TokenDroplist("dynamic options default", true, "{645DF3D7-28AF-4984-90A2-AEA8913DF75A}", true, "{475E9026-333F-432D-A4DC-52E03B75CB6B}")]
		public string DroplistDefaultDynamicOpsIds { get; set; }
		[TokenDroplist("dynamic options default", true, "{645DF3D7-28AF-4984-90A2-AEA8913DF75A}", false, "Marketing Taxonomy")]
		public string DroplistDefaultDynamicOps { get; set; }
		[TokenDroplist("dynamic options no default", true, "{645DF3D7-28AF-4984-90A2-AEA8913DF75A}")]
		public string DroplistDefaultNoDynamicOpsIds { get; set; }
		[TokenDroplist("dynamic options no default", true, "{645DF3D7-28AF-4984-90A2-AEA8913DF75A}", false)]
		public string DroplistDefaultNoDynamicOps { get; set; }

		[TokenGeneralLink("General link no default", true)]
		public GeneralLink NoDefaultLink { get; set; }
		[TokenGeneralLink("General link default url", true, "{645DF3D7-28AF-4984-90A2-AEA8913DF75A}", "http://www.blarp.com")]
		public GeneralLink DefaultLinkUrl { get; set; }
		[TokenGeneralLink("General link default item", true, "{3D6658D8-A0BF-4E75-B3E2-D050FABCF4E1}", "{4B48D87B-E463-42DD-B0F2-F45C382F6204}")]
		public GeneralLink DefaultLinkItem { get; set; }

		[TokenId("id", true)]
		public ID IdNoDefault;
		[TokenId("id default", true, "{3D6658D8-A0BF-4E75-B3E2-D050FABCF4E1}", "{A227A241-3E6C-4841-BB1D-2770FBA46621}")]
		public ID IdDefault;
	}
}
