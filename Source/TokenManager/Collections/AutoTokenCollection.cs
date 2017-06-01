using System.Linq;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Data.Tokens;

namespace TokenManager.Collections
{
	public class AutoTokenCollection : TokenCollection<IToken>
	{
		private readonly string _label;

		public AutoTokenCollection(AutoToken token)
		{
			_label = token.CollectionName;
		} 
		public override IToken InitiateToken(string token)
		{
			return null;
		}

		public override string GetCollectionLabel()
		{
			return _label;
		}

		public override void ResetTokenCache()
		{
		}

		public override bool IsCurrentContextValid(Item item = null)
		{
			return GetTokens().Select(x => x as AutoToken).Where(x => x != null).Any(x => x.IsCurrentContextValid(item));
		}
	}
}
