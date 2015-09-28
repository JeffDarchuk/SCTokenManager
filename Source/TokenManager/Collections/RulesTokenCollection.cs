using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Data.Tokens;
using TokenManager.Management;

namespace TokenManager.Collections
{
    class RulesTokenCollection : SitecoreTokenCollection<IToken>
    {
        public RulesTokenCollection(Item backingItem, ID tokenTemplateID) : base(backingItem, tokenTemplateID)
        {
        }

        public override IToken InitiateToken(string token)
        {
			Database db = TokenKeeper.CurrentKeeper.GetDatabase();
			Item tokenItem = db.GetItem(GetBackingItemId()).Children.FirstOrDefault(i => i["Token"] == token);
			if (tokenItem == null)
				return null;
			return new RuleToken(token, tokenItem.ID);
        }
    }
}
