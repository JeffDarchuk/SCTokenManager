using System;
using Sitecore.Data;
using Sitecore.Diagnostics;
using TokenManager.Collections;

namespace TokenManager.Pipelines.GetTokenCollection
{

	public class GetSitecoreCollection
	{
		/// <summary>
		/// Identifies if the current item in the args is a SitecoreTokenCollection item
		/// </summary>
		/// <param name="args"></param>
		public void Process(GetTokenCollectionTypeArgs args)
		{
			if (args.CollectionItem.TemplateID.ToString() != Constants.TokenCollectionTemplateId) return;
			try
			{
				args.Collection = new SimpleSitecoreTokenCollection(args.CollectionItem, new ID(Constants.TokenTemplateId));
				args.AbortPipeline();
			}
			catch (Exception e)
			{
				Log.Error("Unable to load sitecore based token", e, this);
			}
		}
	}
}
