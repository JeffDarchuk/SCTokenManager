using System.Collections.Generic;

using Sitecore.Data.Items;
using Sitecore.Data;

namespace TokenManager.Data.Interfaces
{

	public interface ITokenCollection
	{
		string this[string token] { get; }

		string GetCollectionLabel();
		IEnumerable<string> GetTokens(); 
		void RemoveToken(string token);
		bool HasToken(string token);
		IToken GetToken(string token);
		void AddOrUpdateToken(string oldToken, IToken newToken);
		void ResetTokenCache();
		ID GetBackingItemId();
		bool IsCurrentContextValid(Item item = null);
	}
}
