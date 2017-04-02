using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes;
using TokenManager.Data.TokenDataTypes.Support;
using TokenManager.Data.TokenExtensions;
using TokenManager.Data.Tokens;

namespace TokenManager
{
	public class tokentest : ViewAutoToken
	{
		public tokentest() : base("test", "people/16x16/cubes_blue.png", "terkan")
		{
		}
		public override TokenButton TokenButton()
		{
			return new Data.TokenExtensions.TokenButton("test", "people/16x16/cubes_blue.png", 1000);
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

		public override object GetModel(TokenDataCollection extraData)
		{
			return extraData;
		}

		public override string GetViewPath(TokenDataCollection extraData)
		{
			return "/views/terkan.cshtml";
		}
	}
}
