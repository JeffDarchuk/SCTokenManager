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
			var collection = TokenIdentifier.Current.ResolveCollection<ITokenCollection<IToken>>(args.CollectionItem);
			if (collection != null)
			{
				args.Collection = collection;
				args.AbortPipeline();
			}
		}
	}
}
