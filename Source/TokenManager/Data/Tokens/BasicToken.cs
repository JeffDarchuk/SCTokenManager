using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.StringExtensions;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

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

		public string TokenIdentifierText(NameValueCollection extraData)
		{
			return "{0} > {1}".FormatWith(extraData["Category"], extraData["Token"]);
		}

		public string TokenIdentifierStyle(NameValueCollection extraData)
		{
			return TokenKeeper.CurrentKeeper.TokenCss;
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
