using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;
using TokenManager.Data.Interfaces;

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
