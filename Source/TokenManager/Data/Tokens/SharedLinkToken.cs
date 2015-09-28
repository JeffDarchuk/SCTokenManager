using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.Data.Fields;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{
    public class SharedLinkToken : IToken
    {
        private readonly ID _backingItem;
        public SharedLinkToken(string token, ID backingItem)
        {
            _backingItem = backingItem;
            Token = token;
        }

        public string Token { get; private set; }

        public string Value(NameValueCollection extraData)
        {
            LinkField f = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(_backingItem).Fields["Link"];
            return string.Format("<a href='{0}' target='{1}' class='{2}' title='{3}'>{4}</a>", f.GetFriendlyUrl(), f.Target,
                f.Class, f.Title, extraData["Text"]);
        }

        public IEnumerable<ITokenData> ExtraData()
        {
            yield return new BasicTokenData("Text", "Link Text", "Enter the text that will appear as the link", true, TokenDataType.String);
        }

        public ID GetBackingItemId()
        {
            return _backingItem;
        }

    }
}
