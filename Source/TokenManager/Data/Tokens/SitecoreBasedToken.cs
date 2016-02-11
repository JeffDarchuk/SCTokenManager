using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using TokenManager.Data.Interfaces;

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
		public ID GetBackingItemId()
		{
			return BackingId;
		}
	}
}
