using System;
using Sitecore.Data;
using Sitecore.Diagnostics;
using TokenManager.Collections;

namespace TokenManager.Pipelines.GetTokenCollection
{
    public class GetRenderingTokenCollection
    {
        /// <summary>
        /// Identifies if the item in the args belongs to a method token collection
        /// </summary>
        /// <param name="args"></param>
        public void Process(GetTokenCollectionTypeArgs args)
        {
	        if (args.CollectionItem.TemplateID.ToString() != Constants.TokenRenderingCollectionTemplateId) return;
	        try
	        {
		        args.Collection = new RenderingTokenCollection(args.CollectionItem,
			        new ID(Constants.TokenRenderingTokenTemplateId)); //rendering token template guid
		        args.AbortPipeline();
	        }
	        catch (Exception e)
	        {
		        Log.Error("unable to load rendering token", e,this);
	        }
        }
    }
}
