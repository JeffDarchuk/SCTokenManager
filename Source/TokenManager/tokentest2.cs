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
	public class tokentest2 : ModelAutoToken<tokentestmodel>
	{
		//Make sure you have a parameterless constructor.
		public tokentest2() : base("test", "people/16x16/cubes_blue.png", "terkan2")
		{
		}

		public override string Render(tokentestmodel model)
		{
			return model.dropy + model.booly + model.stringy;
		}
	}
}
