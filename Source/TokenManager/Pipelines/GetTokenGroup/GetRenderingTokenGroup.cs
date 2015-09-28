using Sitecore.Data;
using TokenManager.Collections;

namespace TokenManager.Pipelines.GetTokenGroup
{
    public class GetRenderingTokenGroup
    {
        /// <summary>
        /// Identifies if the item in the args belongs to a method token group
        /// </summary>
        /// <param name="args"></param>
        public void Process(GetTokenCollectionTypeArgs args)
        {
            if (args.GroupItem.TemplateID.ToString() == Constants._tokenRenderingCollectionTemplateId)
            {
                args.Collection = new RenderingTokenCollection(args.GroupItem, new ID(Constants._tokenRenderingTokenTemplateId));//rendering token template guid
                args.AbortPipeline();
            }
        }
    }
}
