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
	public class tokentest3 : AutoToken
	{
		//Make sure you have a parameterless constructor.
		public tokentest3() : base("test", "people/16x16/cubes_blue.png", "terkan3")
		{
		}

		public override string Value(TokenDataCollection extraData)
		{
			return extraData.GetString("pie");
		}

		public override IEnumerable<ITokenData> ExtraData()
		{
			yield return new StringTokenData("pie", "pie", "pie", true);
		}
	}
}
