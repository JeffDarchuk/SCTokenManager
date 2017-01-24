using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;

namespace TokenManager.Management
{
	public class TokenIdentifier
	{
		internal static ITokenIdentifier IdentifierSingleton;
		public static ITokenIdentifier Current => IdentifierSingleton;
	}
}
