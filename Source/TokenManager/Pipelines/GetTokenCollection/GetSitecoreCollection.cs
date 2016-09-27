using System;
using Sitecore.Data;
using Sitecore.Diagnostics;
using TokenManager.Collections;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

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
			try
			{
				var collection = TokenIdentifier.Current.ResolveCollection<ITokenCollection<IToken>>(args.CollectionItem);
				if (collection != null)
				{
					args.Collection = collection;
					args.AbortPipeline();
				}
			}
			catch (Exception e)
			{
				Log.Error("Issue resolving token collection " + args.CollectionItem + " this could be due to the link database needing to be rebuilt or a database cleanup operation needs to be run", e, this);
			}
		}
	}
}
