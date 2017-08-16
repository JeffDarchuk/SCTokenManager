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
	public class DemoAutoToken2 : AutoToken<DemoAutoToken2Model>
	{
		//Make sure you have a parameterless constructor.
		public DemoAutoToken2() : base(collectionName: "Demo Collection 2", tokenIcon: "people/16x16/cubes_blue.png", tokenName:"Demo Token")
		{
		}
		/// <summary>
		/// Render the token value given your requested model.
		/// </summary>
		/// <param name="model">The model you are to use to build the token value.</param>
		/// <returns>Token Value</returns>
		public override string Render(DemoAutoToken2Model model)
		{
			return model.MySecretValue;
		}
	}
}
