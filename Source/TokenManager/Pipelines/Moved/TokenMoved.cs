using System;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events;

using TokenManager.Handlers;
using TokenManager.Management;

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
				TokenIncorporator inc = new TokenIncorporator(toItem["Category Label"], item["Token"], TokenKeeper.CurrentKeeper.GetTokenIdentifier(fromItem["Category Label"], item["Token"]));
				inc.Incorporate();
			}
			else if (fromItem.TemplateID.ToString() == Constants._tokenGroupTemplateId)
			{
				TokenUnzipper unzipper = new TokenUnzipper(fromItem["Category Label"], item["Token"]);
				unzipper.Unzip();
			}
		}
	}
}
