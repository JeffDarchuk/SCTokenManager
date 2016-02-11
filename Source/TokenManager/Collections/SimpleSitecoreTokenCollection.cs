using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Data.Tokens;
using TokenManager.Management;

namespace TokenManager.Collections
{
	public class SimpleSitecoreTokenCollection : SitecoreTokenCollection<IToken>
	{
		private readonly ID _backingItemId;
		public SimpleSitecoreTokenCollection(Item tokenCollection)
			: base(tokenCollection)
		{
			_backingItemId = tokenCollection.ID;
		}
		/// <summary>
		/// loads in the token to the collection
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public override IToken InitiateToken(string token)
		{
			return TokenIdentifier.Current.ResolveToken<IToken>(this, token);
		}
	}
}
