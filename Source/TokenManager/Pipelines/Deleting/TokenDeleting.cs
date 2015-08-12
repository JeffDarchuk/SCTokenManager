using System;

using Sitecore.Data.Items;
using Sitecore.Events;
using TokenManager.Data.Interfaces;
using TokenManager.Handlers;
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
			if (item.TemplateID.ToString() == Constants._tokenTemplateId)
			{
				TokenUnzipper unzipper = new TokenUnzipper(item.Parent["Category Label"], item["Token"]);
				unzipper.Unzip();
				var sitecoreTokenCollection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(item.Parent["Category Label"]);
				if (sitecoreTokenCollection != null) 
					sitecoreTokenCollection.RemoveToken(item["Token"]);
			}
			else if (item.TemplateID.ToString() == Constants._tokenGroupTemplateId || item.TemplateID.ToString() == Constants._tokenMethodGroupTemplateId)
			{
				var sitecoreTokenCollection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(item["Category Label"]);
			    if (sitecoreTokenCollection != null)
			    {
			        foreach (var token in sitecoreTokenCollection.GetTokens())
			        {
			            TokenUnzipper unzipper = new TokenUnzipper(item["Category Label"], token);
			            unzipper.Unzip();
			        }
			        TokenKeeper.CurrentKeeper.RemoveGroup(item["Category Label"]);
			    }
			}
		}
	}
}
