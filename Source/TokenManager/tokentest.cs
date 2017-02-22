using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes;
using TokenManager.Data.TokenDataTypes.Support;
using TokenManager.Data.Tokens;

namespace TokenManager
{
	public class tokentest : AutoToken
	{
		public tokentest() : base("test", "people/16x16/cubes_blue.png", "terkan")
		{
		}

		public override string Value(TokenDataCollection extraData)
		{
			return $@"
<div>{extraData.GetLink("link").LinkDescription}</div>
<div>{extraData.GetDropdownValue("dd")}</div>
<div>{extraData.GetBoolean("bool")}</div>
<div>{extraData.GetId("id")}</div>
<div>{extraData.GetInt("int")}</div>
";
		}

		public override IEnumerable<ITokenData> ExtraData()
		{
			yield return new GeneralLinkTokenData("LINK", "link", true);
			yield return new DroplistTokenData("dd", "dd", true, new []
			{
				new KeyValuePair<string, string>("test", "test"),
				new KeyValuePair<string, string>("monkey", "blue"),
			});
			yield return new BooleanTokenData("bool", "bool");
			yield return new IdTokenData("id", "id", true);
			yield return new IntegerTokenData("int", "int", true);
		}
	}
}
