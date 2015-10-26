using System;
using System.Reflection;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using TokenManager.Data.Interfaces;

namespace TokenManager.Pipelines.GetTokenCollection
{

	public class GetDynamicCollection
	{
		/// <summary>
		/// Identifies if the item in the args belongs to a method token collection
		/// </summary>
		/// <param name="args"></param>
		public void Process(GetTokenCollectionTypeArgs args)
		{
			if (args.CollectionItem.TemplateID.ToString() == Constants.TokenMethodCollectionTemplateId)
			{
				if (string.IsNullOrWhiteSpace(args.CollectionItem["type"]))
				{
					args.AbortPipeline();
					return;
				}
			    try
			    {
			        string[] tmp = args.CollectionItem["type"].Split(',');
			        Type t = Assembly.Load(tmp[1]).GetType(tmp[0]);
			        ConstructorInfo ctor = t.GetConstructor(new[] {typeof (Item)});
				    if (ctor == null) return;
				    args.Collection = (ITokenCollection<IToken>) ctor.Invoke(new object[] {args.CollectionItem});
				    args.AbortPipeline();
			    }
			    catch (Exception e)
			    {
			        Log.Error("Unable to resolve dynamic token type", e, this);
			    }
			}
		}
	}
}
