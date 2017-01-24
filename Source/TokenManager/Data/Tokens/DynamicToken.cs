using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.StringExtensions;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{

	public class DynamicToken : IToken
	{
		private readonly Func<string> _valueFunc;

		/// <summary>
		/// set up the dynamic token with the func to evaluate
		/// </summary>
		/// <param name="token"></param>
		/// <param name="valueFunc"></param>
		public DynamicToken(string token, Func<string> valueFunc)
		{
			Token = token;
			_valueFunc = valueFunc;
		}
		public string Token { get; set; }

		public string Value(NameValueCollection extraData)
		{
			return _valueFunc();
		}

		public IEnumerable<ITokenData> ExtraData()
		{
			return null;
		}

		public virtual string TokenIdentifierText(NameValueCollection extraData)
		{
			return "{0} > {1}".FormatWith(extraData["Category"], extraData["Token"]);
		}

		public virtual string TokenIdentifierStyle(NameValueCollection extraData)
		{
			return TokenKeeper.CurrentKeeper.TokenCss;
		}

		public ID GetBackingItemId()
		{
			return null;
		}

		public IToken LoadExtraData(NameValueCollection props)
		{
			return this;
		}
	}
}
