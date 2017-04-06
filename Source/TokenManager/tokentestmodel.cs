using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.TokenDataTypes.Attributes;

namespace TokenManager
{
	public class tokentestmodel
	{
		[TokenBoolean("THIS IS A TEST")]
		public bool booly;
		[TokenDroplist("DROPY", true, new[] { "test", "TEST", "floo", "FLARM" })]
		public string dropy;
		[TokenDroplist("DROPY", true, "{62BC5816-165B-413D-B8E7-692063D12374}", false)]
		public string dropy2;
		[TokenGeneralLink("LINKEYLINK", true, "{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}")]
		public string linky;
		[TokenId("ID N STUFF", true)]
		public ID idy;
		[TokenInteger("INT!", true)]
		public int inty;
		[TokenString("STRINGINGINGING", "PLACEHOLDER", true)]
		public string stringy;
	}
}
