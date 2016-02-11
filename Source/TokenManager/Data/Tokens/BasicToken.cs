using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data
{

	public class BasicToken : IToken
	{
		public string Token { get; set; }

		public string Value { get; set; }

		string IToken.Value(NameValueCollection extraData)
		{
			return Value;
		}

		public IEnumerable<ITokenData> ExtraData()
		{
			return null;
		}

		public ID GetBackingItemId()
		{
			return null;
		}

		public BasicToken(string token, string value)
		{
			Value = value;
			Token = token;

		}
	}
}
