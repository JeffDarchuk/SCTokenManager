using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.Data.TokenDataTypes.Support
{
	public class TokenCastException : Exception
	{
		public TokenCastException(string message) : base(message) { }
	}
}
