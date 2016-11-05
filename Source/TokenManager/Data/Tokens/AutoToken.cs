using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.StringExtensions;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{
	public abstract class AutoToken : IToken
	{
		protected AutoToken(string collectionName, string tokenIcon, string tokenName)
		{
			this.CollectionName = collectionName;
			this.TokenIcon = tokenIcon;
			this.Token = tokenName;
		}
		public string CollectionName { get; }
		public string TokenIcon { get; }
		public string Token { get; }
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

		public virtual ID GetBackingItemId()
		{
			return default(ID);
		}

		public virtual IEnumerable<ID> ValidTemplates()
		{
			yield break;
		}

		public virtual IEnumerable<ID> ValidParents()
		{
			yield break;
		}
		public virtual bool IsCurrentContextValid(Item item = null)
		{
			if (item == null)
			{
				item = Context.Item;
			}
			if (item == null || item.Database.Name != TokenKeeper.CurrentKeeper.GetDatabase().Name)
				return true;

			if (!IsAllowed(item))
				return false;

			return true;
		}

		private bool IsAllowed(Item tokenTarget)
		{
			var validTemplates = ValidTemplates().ToList();
			var validParents = ValidParents().ToList();

			if (validParents.Any() && !validParents
					.Any(x => IsAllowedForItem(tokenTarget, tokenTarget.Database.GetItem(x))))
				return false;
			if (validTemplates.Any() && validTemplates.All(x => x != tokenTarget.TemplateID))
				return false;
				
			return true;
		}

		private static bool IsAllowedForItem(Item tokenTarget, Item tmp)
		{
			var parentFilterPath = "";
			if (tmp != null)
				parentFilterPath = tmp.Paths.Path;
			if (!string.IsNullOrWhiteSpace(parentFilterPath) && !tokenTarget.Paths.Path.StartsWith(parentFilterPath))
				return false;
			return true;
		}
	}
}
