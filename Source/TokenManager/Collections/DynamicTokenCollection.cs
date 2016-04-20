using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Data.Tokens;

namespace TokenManager.Collections
{
	public abstract class DynamicTokenCollection : TokenCollection<IToken>
	{
		public new abstract IEnumerable<DynamicToken> GetTokens();
		protected DynamicTokenCollection(Item backingItem) : base(backingItem)
		{
			GatherTokens();
		}

		private void GatherTokens()
		{
			foreach(DynamicToken token in GetTokens())
				AddOrUpdateToken(token);
		}

		public override IToken InitiateToken(string token)
		{
			throw new NotImplementedException();
		}

		public override void ResetTokenCache()
		{
			throw new NotImplementedException();
		}
	}
}
