using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;

namespace TokenManager.Data.Interfaces
{
	public interface IToken
	{
		string Token { get; }
		string Value(NameValueCollection extraData);
		IEnumerable<ITokenData> ExtraData();
		string TokenIdentifierText(NameValueCollection extraData);
		string TokenIdentifierStyle(NameValueCollection extraData);
		ID GetBackingItemId();
	}
}
