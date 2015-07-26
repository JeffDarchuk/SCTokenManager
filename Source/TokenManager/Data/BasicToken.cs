using Sitecore.Data;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data
{

	public class BasicToken : IToken
	{
		public string Token { get; set; }

		public string Value { get; set; }
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
