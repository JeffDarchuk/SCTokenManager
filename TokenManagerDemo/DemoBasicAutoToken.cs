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
	public class DemoBasicAutoToken : AutoToken
	{
		//Make sure you have a parameterless constructor.
		public DemoBasicAutoToken() : base( collectionName: "Demo Collection", tokenIcon: "people/16x16/cubes_blue.png", tokenName:"Demo Basic Token")
		{
		}

		public override TokenButton TokenButton()
		{
			return new TokenButton("Basic STUFS", "people/16x16/cubes_blue.png", 1000);
		}

		/// <summary>
		/// Returns the value for the token that is then injected into the RTE.
		/// </summary>
		/// <param name="extraData">The user defined data that was gathered when the author added the token to the RTE.</param>
		/// <returns>Token Value</returns>
		public override string Value(TokenDataCollection extraData)
		{
			return extraData["mystring"];
		}

		/// <summary>
		/// Defines what extra data to collect from the user when the token was inserted into the RTE.
		/// </summary>
		/// <returns>List of token data types to collect from the user when the token was inserted into the RTE.</returns>
		public override IEnumerable<ITokenData> ExtraData()
		{
			yield break;
		}
	}
}
