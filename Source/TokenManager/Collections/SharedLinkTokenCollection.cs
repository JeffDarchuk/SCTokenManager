using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Data.Tokens;
using TokenManager.Management;

namespace TokenManager.Collections
{
    public class SharedLinkTokenCollection : SitecoreTokenCollection<IToken>
    {
        public SharedLinkTokenCollection(Item tokenCollection, ID tokenTemplateID) : base(tokenCollection, tokenTemplateID)
        {
        }

        public override IToken InitiateToken(string token)
        {
            Database db = TokenKeeper.CurrentKeeper.GetDatabase(); ;
            Item tokenItem = db.GetItem(GetBackingItemId()).Children.FirstOrDefault(i => i["Token"] == token);
            if (tokenItem == null)
                return null;
            return new SharedLinkToken(token, tokenItem.ID);
        }
    }
}
