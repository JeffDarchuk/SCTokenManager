using System.Collections.Generic;

using Sitecore.Data.Items;
using Sitecore.Data;

namespace TokenManager.Data.Interfaces
{

	public interface ITokenCollection<T>
        where T : IToken
	{
		string this[string token] { get; }

		string GetCollectionLabel();
		IEnumerable<string> GetTokens(); 
		void RemoveToken(string token);
		bool HasToken(string token);
		T GetToken(string token);
		void AddOrUpdateToken(string oldToken, T newToken);
		void ResetTokenCache();
		ID GetBackingItemId();
		bool IsCurrentContextValid(Item item = null);
	}
}
