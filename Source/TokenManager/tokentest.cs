﻿using System;
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
	public class tokentest : ViewAutoToken<tokentestmodel>
	{
		//Make sure you have a parameterless constructor.
		public tokentest() : base("test", "people/16x16/cubes_blue.png", "terkan")
		{
		}
		public override string GetViewPath(tokentestmodel extraData)
		{
			return "/views/terkan.cshtml";
		}
	}
}
