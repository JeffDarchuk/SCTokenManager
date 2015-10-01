using System;
using Sitecore.Data;
using Sitecore.Diagnostics;
using TokenManager.Collections;

namespace TokenManager.Pipelines.GetTokenGroup
{
	class GetRuleTokenGroup
	{
		/// <summary>
		/// Identifies if the current item in the args is a SitecoreTokenCollection item
		/// </summary>
		/// <param name="args"></param>
		public void Process(GetTokenCollectionTypeArgs args)
		{
			if (args.GroupItem.TemplateID.ToString() == "{147C5339-DB74-413F-A749-AE4C29B7C1F0}")//rules token collection template guid
			{
			    try
			    {
			        args.Collection = new RulesTokenCollection(args.GroupItem, new ID("{7D6A72CC-D346-43D2-A279-EF6CA27BCF31}"));
			        //rules token template guid
			        args.AbortPipeline();
			    }
			    catch (Exception e)
			    {
			        Log.Error("Unable to load rules token", e, this);
			    }
			}
		}
	}
}
