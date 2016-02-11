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
	class RuleToken : SitecoreBasedToken
	{

		public RuleToken(string name, ID backingId) : base(name, backingId)
		{
		}
		public override string Value(NameValueCollection extraData)
		{
			Database db = TokenKeeper.CurrentKeeper.GetDatabase();
			Item item = db.GetItem(BackingId);
			if (item != null)
			{
				TokenRuleContext context = new TokenRuleContext();
				item.RunRules("Value", context);
				return context.Value;
			}
			return string.Empty;
		}

		public override IEnumerable<ITokenData> ExtraData()
		{
			return null;
		}

		public IToken LoadExtraData(NameValueCollection props)
		{
			return this;
		}

	}
}
