using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events;
using TokenManager.Handlers.TokenOperations;

namespace TokenManager.Pipelines.Moved
{

	class TokenMoved
	{
		/// <summary>
		/// Handles if a token gets moved out of a group.  Either it gets re-incorporated into another group or unzipped
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnItemMoved(object sender, EventArgs e)
		{
			var item = Event.ExtractParameter<Item>(e, 0);
			if (item.TemplateID.ToString() != Constants._tokenTemplateId) return;
			var from = Event.ExtractParameter<ID>(e, 1);
			var fromItem = item.Database.GetItem(from);
			var toItem = item.Parent;
			if (fromItem.TemplateID.ToString() == Constants._tokenGroupTemplateId &&
			    toItem.TemplateID.ToString() == Constants._tokenGroupTemplateId)
			{
                TokenRootPropertyChanger changer = new TokenRootPropertyChanger(fromItem["Category Label"], item["Token"]);
			    changer.Change(toItem["Category Label"], item["Token"]);
			}
			else if (fromItem.TemplateID.ToString() == Constants._tokenGroupTemplateId)
			{
				TokenUnzipper unzipper = new TokenUnzipper("{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}",fromItem["Category Label"], item["Token"], true);
				unzipper.Unzip();
			}
		}
	}
}
