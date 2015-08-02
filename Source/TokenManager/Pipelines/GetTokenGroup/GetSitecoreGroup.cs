using Sitecore.Data;
using TokenManager.Collections;
using TokenManager.Data;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Pipelines.GetTokenGroup
{

	public class GetSitecoreGroup
	{
		/// <summary>
		/// Identifies if the current item in the args is a SitecoreTokenCollection item
		/// </summary>
		/// <param name="args"></param>
		public void Process(GetTokenCollectionTypeArgs args)
		{
			if (args.GroupItem.TemplateID.ToString() == Constants._tokenGroupTemplateId)
			{
				args.Collection = new SimpleSitecoreTokenCollection(args.GroupItem, new ID(Constants._tokenTemplateId));
				args.AbortPipeline();
			}
		}
	}
}
