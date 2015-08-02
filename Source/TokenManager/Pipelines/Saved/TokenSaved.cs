using System;
using System.Linq;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Events;

using TokenManager.Data.Interfaces;
using TokenManager.Handlers;
using TokenManager.Management;

namespace TokenManager.Pipelines.Saved
{

	public class TokenSaved
	{
		/// <summary>
		/// Use the TokenIncorporator to update all existing useages of the changed token to be the new format based on the save
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnItemSaved(object sender, EventArgs e)
		{
			var item = Event.ExtractParameter<Item>(e, 0);
			var changes = Event.ExtractParameter<ItemChanges>(e, 1);

			if (item != null && changes != null)
			{
				// if it's a specific token that's changed
				if (TemplateManager.GetTemplate(item).IsDerived(new ID(Constants._tokenTemplateBaseId)))
					foreach (ITokenCollection<IToken> tokenCollection in TokenKeeper.CurrentKeeper.GetTokenCollections()
						.Where(x => x.GetBackingItemId() == item.ParentID))
					{
						foreach (FieldChange change in changes.FieldChanges)
						{
							if (change.FieldID.ToString() == Constants._tokenFieldId && change.OriginalValue != "$name")
							{
								var inc = new TokenIncorporator(item.Parent["Category Label"], item["Token"],
									TokenKeeper.CurrentKeeper.GetTokenIdentifier(item.Parent["Category Label"], change.OriginalValue));
								inc.Incorporate();
							}
						}
						tokenCollection.ResetTokenCache();
					}
				else
				{
					foreach (FieldChange change in changes.FieldChanges)
					{
						if (item.Fields[change.FieldID].Type.ToLower() == "rich text")
						{
							TokenKeeper.CurrentKeeper.ResetTokenLocations(item.ID, change.FieldID);
						}
					}
				}
				// if it's a token collection that's changed
				if (TemplateManager.GetTemplate(item).IsDerived(new ID(Constants._tokenGroupTemplateBaseId)))
				{
					string oldCategoryName = item["Category Label"];
					foreach (FieldChange change in changes.FieldChanges)
					{
						if (change.FieldID.ToString() == Constants._tokenGroupCategoryFieldId)
						{
							var tokenCollection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(change.OriginalValue);
							if (tokenCollection != null)
							{
								TokenKeeper.CurrentKeeper.LoadTokenGroup(TokenKeeper.CurrentKeeper.GetCollectionFromItem(item));
								foreach (var token in tokenCollection.GetTokens())
								{
									var inc = new TokenIncorporator(item["Category Label"], token,
										TokenKeeper.CurrentKeeper.GetTokenIdentifier(change.OriginalValue, token));
									inc.Incorporate();
								}
								oldCategoryName = change.OriginalValue;

							}
						}
					}
					TokenKeeper.CurrentKeeper.RemoveGroup(oldCategoryName);
					if (item["Category Label"] == oldCategoryName)
						TokenKeeper.CurrentKeeper.LoadTokenGroup(TokenKeeper.CurrentKeeper.GetCollectionFromItem(item));
				}


			}
		}
	}
}
