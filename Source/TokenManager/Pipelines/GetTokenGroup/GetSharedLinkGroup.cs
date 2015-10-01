using System;
using Sitecore.Data;
using Sitecore.Diagnostics;
using TokenManager.Collections;

namespace TokenManager.Pipelines.GetTokenGroup
{
    public class GetSharedLinkGroup
    {
        /// <summary>
        /// Identifies if the item in the args belongs to a method token group
        /// </summary>
        /// <param name="args"></param>
        public void Process(GetTokenCollectionTypeArgs args)
        {
            if (args.GroupItem.TemplateID.ToString() == Constants._tokenSharedLinkCollectionTemplateId)
            {
                try
                {
                    args.Collection = new SharedLinkTokenCollection(args.GroupItem,
                        new ID(Constants._tokenSharedLinkTemplateId)); //rules token template guid
                    args.AbortPipeline();
                }
                catch (Exception e)
                {
                    Log.Error("Unable to load shared link token" , e, this);
                }
            }
        }
    }
}
