using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.Interfaces
{
	interface ITokenDataAttribute
	{
		ITokenData TokenData { get; }
		object GetObject(TokenDataCollection collection, string name, Type t);
	}
}
