using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events;
using TokenManager.Data.Interfaces;
using TokenManager.Handlers.TokenOperations;
using TokenManager.Management;

namespace TokenManager.Pipelines.Moved
{

	class TokenMoved
	{
		/// <summary>
		/// Handles if a token gets moved out of a collection.  If it's a moved to a similar parent it'll be adopted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnItemMoved(object sender, EventArgs e)
		{
			var item = Event.ExtractParameter<Item>(e, 0);
			if (!item.Template.IsDerived(new ID(Constants.TokenTemplateBaseId))) return;
			var from = Event.ExtractParameter<ID>(e, 1);
			var fromItem = item.Database.GetItem(from);
			var toItem = item.Parent;
			var fromCollection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(from);
			var toCollection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(toItem.ID);
			if (fromCollection != null && toCollection != null)
			{
				var token = fromCollection.GetTokens().FirstOrDefault(x => x.GetBackingItemId() == item.ID);
				if (token != null)
				{
					TokenRootPropertyChanger changer = new TokenRootPropertyChanger(fromCollection.GetCollectionLabel(), token.Token);
					changer.Change(toCollection.GetCollectionLabel(), token.Token);
				}
			}
			fromCollection?.ResetTokenCache();
			toCollection?.ResetTokenCache();
		}
	}
}
