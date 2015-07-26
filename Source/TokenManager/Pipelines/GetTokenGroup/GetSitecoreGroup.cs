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
				args.Collection = new SitecoreTokenCollection(args.GroupItem);
				args.AbortPipeline();
			}
		}
	}
}
