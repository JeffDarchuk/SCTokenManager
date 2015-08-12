using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Collections
{
	public class SimpleSitecoreTokenCollection : SitecoreTokenCollection<IToken>
	{
		private readonly ID _backingItemId;
		public SimpleSitecoreTokenCollection(Item tokenGroup, ID tokenTemplateID)
			: base(tokenGroup, tokenTemplateID)
		{
			_backingItemId = tokenGroup.ID;
		}
		/// <summary>
		/// loads in the token to the collection
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public override IToken InitiateToken(string token)
		{
			Database db = GetDatabase();
			Item tokenItem = db.GetItem(_backingItemId).Children.FirstOrDefault(i => i["Token"] == token);
			if (tokenItem == null)
				return null;
			return new SitecoreToken(token, tokenItem.ID);
		}
	}
}
