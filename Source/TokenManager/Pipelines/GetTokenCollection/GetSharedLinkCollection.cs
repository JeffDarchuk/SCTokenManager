using System;
using Sitecore.Data;
using Sitecore.Diagnostics;
using TokenManager.Collections;

namespace TokenManager.Pipelines.GetTokenCollection
{
    public class GetSharedLinkCollection
    {
        /// <summary>
        /// Identifies if the item in the args belongs to a method token collection
        /// </summary>
        /// <param name="args"></param>
        public void Process(GetTokenCollectionTypeArgs args)
        {
	        if (args.CollectionItem.TemplateID.ToString() != Constants.TokenSharedLinkCollectionTemplateId) return;
	        try
	        {
		        args.Collection = new SharedLinkTokenCollection(args.CollectionItem,
			        new ID(Constants.TokenSharedLinkTemplateId)); //rules token template guid
		        args.AbortPipeline();
	        }
	        catch (Exception e)
	        {
		        Log.Error("Unable to load shared link token" , e, this);
	        }
        }
    }
}
