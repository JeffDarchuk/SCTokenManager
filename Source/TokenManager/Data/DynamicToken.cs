using System;
using Sitecore.Data;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data
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
		string IToken.Value
		{
			get
			{
				return _valueFunc();
			}
		}

		public ID GetBackingItemId()
		{
			return null;
		}
	}
}
