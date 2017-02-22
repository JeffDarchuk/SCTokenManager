using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.Interfaces
{
	public interface IToken
	{
		string Token { get; }
		string Value(TokenDataCollection extraData);
		IEnumerable<ITokenData> ExtraData();
		string TokenIdentifierText(TokenDataCollection extraData);
		string TokenIdentifierStyle(TokenDataCollection extraData);
		ID GetBackingItemId();
	}
}
