using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Management;
using TokenManager.Rules;

namespace TokenManager.Data.Tokens
{
    class RuleToken : IToken
    {
		private ID _backingItem;

		public string Token { get; set; }

		public RuleToken(string token, ID scId)
		{
			_backingItem = scId;
			Token = token;
		}

        public string Value(NameValueCollection extraData)
        {
	        Database db = TokenKeeper.CurrentKeeper.GetDatabase();
            Item item = db.GetItem(_backingItem);
            if (item != null)
            {
                TokenRuleContext context = new TokenRuleContext();
                item.RunRules("Value", context);
                return context.Value;
            }
            return string.Empty;
        }

        public IEnumerable<ITokenData> ExtraData()
        {
            return null;
        }

        public ID GetBackingItemId()
	    {
		    return _backingItem;
	    }

        public IToken LoadExtraData(NameValueCollection props)
        {
            return this;
        }
    }
}
