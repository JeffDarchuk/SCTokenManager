using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Events;
using TokenManager.Data.Interfaces;
using TokenManager.Handlers.TokenOperations;
using TokenManager.Management;

namespace TokenManager.Pipelines.Deleting
{

	public class TokenDeleting
	{
		/// <summary>
		/// Handles a token being deleted, either unzips the token or the entire token group
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnItemDeleting(object sender, EventArgs e)
		{
			var item = Event.ExtractParameter<Item>(e, 0);
			if (item == null) return;
            if (item.Template.IsDerived(new ID(Constants._tokenTemplateBaseId)))
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
			        if (collection != null)
			        {
			            var token = collection.GetTokens().FirstOrDefault(x => x.GetBackingItemId() == item.ID);
			            if (token != null)
			            {
			                TokenUnzipper unzipper = new TokenUnzipper("{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}",
			                    collection.GetCollectionLabel(), token.Token, true);
			                unzipper.Unzip();
                            collection.RemoveToken(token.Token);
			            }
			            else
			            {
			                collection.ResetTokenCache();
			            }
			        }
			        
			    }
			}
            else if (item.Template.IsDerived(new ID(Constants._tokenGroupTemplateBaseId)))
			{
				var sitecoreTokenCollection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(item.ID);
			    if (sitecoreTokenCollection != null)
			    {
			        foreach (var token in sitecoreTokenCollection.GetTokens())
			        {
			            TokenUnzipper unzipper = new TokenUnzipper("{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}",sitecoreTokenCollection.GetCollectionLabel(), token.Token, true);
			            unzipper.Unzip();
			        }
			        TokenKeeper.CurrentKeeper.RemoveGroup(sitecoreTokenCollection.GetCollectionLabel());
			    }
			}
		}
	}
}
