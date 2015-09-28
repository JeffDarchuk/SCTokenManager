using System;
using System.Reflection;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;

namespace TokenManager.Pipelines.GetTokenGroup
{

	public class GetDynamicGroup
	{
		/// <summary>
		/// Identifies if the item in the args belongs to a method token group
		/// </summary>
		/// <param name="args"></param>
		public void Process(GetTokenCollectionTypeArgs args)
		{
			if (args.GroupItem.TemplateID.ToString() == Constants._tokenMethodGroupTemplateId)
			{
				if (string.IsNullOrWhiteSpace(args.GroupItem["type"]))
				{
					args.AbortPipeline();
					return;
				}
				string[] tmp = args.GroupItem["type"].Split(',');
				Type t = Assembly.Load(tmp[1]).GetType(tmp[0]);
				ConstructorInfo ctor = t.GetConstructor(new[] { typeof(Item) });
				if (ctor != null)
				{
					args.Collection= (ITokenCollection<IToken>)ctor.Invoke(new object[] { args.GroupItem });
					args.AbortPipeline();
				}
			}
		}
	}
}
