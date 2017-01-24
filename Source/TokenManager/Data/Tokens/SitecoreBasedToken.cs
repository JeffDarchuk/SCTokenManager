using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.StringExtensions;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{
	public abstract class SitecoreBasedToken : IToken
	{
		protected string TokenName;
		protected ID BackingId;
		public SitecoreBasedToken(string name, ID backingId)
		{
			BackingId = backingId;
			TokenName = name;
		}

		public string Token => TokenName;
		public abstract string Value(NameValueCollection extraData);
		public abstract IEnumerable<ITokenData> ExtraData();
		public virtual string TokenIdentifierText(NameValueCollection extraData)
		{
			return "{0} > {1}".FormatWith(extraData["Category"], extraData["Token"]);
		}

		public virtual string TokenIdentifierStyle(NameValueCollection extraData)
		{
			return TokenKeeper.CurrentKeeper.TokenCss;
		}

		public ID GetBackingItemId()
		{
			return BackingId;
		}
	}
}
