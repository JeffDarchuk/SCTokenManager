using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Events;
using TokenManager.Data.Interfaces;
using TokenManager.Handlers.TokenOperations;
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

            if (item != null && changes != null && !item.Paths.FullPath.StartsWith("/sitecore/templates"))
			{
				// if it's a specific token that's changed
			    if (TemplateManager.GetTemplate(item).IsDerived(new ID(Constants._tokenTemplateBaseId)))
			    {
			        var parent = item.Parent;
			        while (parent != null &&
			               !TemplateManager.GetTemplate(parent).IsDerived(new ID(Constants._tokenGroupTemplateBaseId)))
			        {
			            parent = parent.Parent;
			        }
			        if (parent != null)
			        {
			            var collection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(parent.ID);
			            IToken token = collection.GetTokens().FirstOrDefault(x => x.GetBackingItemId() == item.ID);
                        collection.ResetTokenCache();
                        IToken newToken = collection.GetTokens().FirstOrDefault(x => x.GetBackingItemId() == item.ID);
			            if (token != null && newToken != null && newToken.Token != token.Token)
			            {
                            TokenRootPropertyChanger changer = new TokenRootPropertyChanger(collection.GetCollectionLabel(), token.Token);
                            changer.Change(collection.GetCollectionLabel(), newToken.Token);
			            }
			        }
			    }
				// if it's a token collection that's changed
				else if (TemplateManager.GetTemplate(item).IsDerived(new ID(Constants._tokenGroupTemplateBaseId)))
				{
				    var collection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(item.ID);
				    if (collection != null)
				    {
				        var newCollection = TokenKeeper.CurrentKeeper.GetCollectionFromItem(item);
				        if (newCollection != null && collection.GetCollectionLabel() != newCollection.GetCollectionLabel())
				        {
				            foreach (var token in collection.GetTokens())
				            {
				                TokenRootPropertyChanger changer = new TokenRootPropertyChanger(collection.GetCollectionLabel(),
				                    token.Token);
				                changer.Change(newCollection.GetCollectionLabel(), token.Token);
				            }
				        }
                        TokenKeeper.CurrentKeeper.RemoveGroup(collection.GetCollectionLabel());
                        TokenKeeper.CurrentKeeper.LoadTokenGroup(newCollection);
				    }
				    else
				    {
                        var newCollection = TokenKeeper.CurrentKeeper.GetCollectionFromItem(item);
                        TokenKeeper.CurrentKeeper.LoadTokenGroup(newCollection);
				    }
				}
                else
                {
                    foreach (FieldChange change in changes.FieldChanges)
                    {
                        if (item.Fields[change.FieldID].Type.ToLower() == "rich text")
                        {
                            TokenKeeper.CurrentKeeper.ResetTokenLocations(item.ID, change.FieldID, item.Language, item.Version.Number);
                        }
                    }
                }
			}
		}
	}
}
