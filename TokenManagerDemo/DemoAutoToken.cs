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
	public class DemoAutoToken : AutoToken
	{
		//Make sure you have a parameterless constructor.
		public DemoAutoToken() : base( collectionName: "Demo Collection", tokenIcon: "people/16x16/cubes_blue.png", tokenName:"Demo Token")
		{
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
			yield return new StringTokenData(label: "Give us a string", name:"mystring",  placeholder: "Enter string here", required: true);
			yield return new IntegerTokenData(label: "Give us an integer", name:"myint", required:true);
			yield return new BooleanTokenData(label: "Give us a boolean", name:"mybool");
			yield return new IdTokenData(label: "Give us a sitecore Item", name: "myitem", required:true, root: "{A227A241-3E6C-4841-BB1D-2770FBA46621}");
			yield return new DroplistTokenData(label: "Give us an option", name: "myoption", required:true, options: new []
			{
				new KeyValuePair<string, string>("Option 1", "op1"), 
				new KeyValuePair<string, string>("Option 2", "op2")
			});
			yield return new GeneralLinkTokenData(label: "Give us a link", name: "mylink", root: "{A227A241-3E6C-4841-BB1D-2770FBA46621}", required: true);
		}
	}
}
