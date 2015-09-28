using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace TokenManager.Data.Interfaces
{

	public interface ITokenCollection<T>
        where T : IToken
	{
		T this[string token] { get; }
        string SitecoreIcon { get; }
		string GetCollectionLabel();
		IEnumerable<IToken> GetTokens(); 
		void RemoveToken(string token);
		bool HasToken(string token);
		T GetToken(string token);
		void AddOrUpdateToken(string oldToken, T newToken);
		void ResetTokenCache();
		ID GetBackingItemId();
		bool IsCurrentContextValid(Item item = null);
	}
}
