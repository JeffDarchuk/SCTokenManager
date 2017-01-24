using System;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.WordOCX;
using TokenManager.Management;

namespace TokenManager.Pipelines.RenderField
{

	class RenderFieldExpandTokens
	{
		/// <summary>
		/// Replaces the field value with the value with the token values injected
		/// </summary>
		/// <param name="args"></param>
		public void Process(RenderFieldArgs args)
		{
			Assert.ArgumentNotNull((object)args, "args");
			string fieldValue = args.FieldValue;
			if (string.IsNullOrWhiteSpace(args.Result.FirstPart))
				args.Result.FirstPart = string.IsNullOrEmpty(fieldValue) ? args.Item[args.FieldName] : fieldValue;
			if (args.FieldTypeKey != "rich text")
				return;
			try
			{
				if (Context.PageMode.IsNormal || Context.PageMode.IsPreview)
					args.Result.FirstPart = TokenKeeper.CurrentKeeper.ReplaceRTETokens(args, args.Result.FirstPart);
			}
			catch (MethodAccessException e)
			{
				args.Result.FirstPart = TokenKeeper.CurrentKeeper.ReplaceRTETokens(args, args.Result.FirstPart);
			}
			WordFieldValue wordFieldValue = WordFieldValue.Parse(args.Result.FirstPart);
			if (wordFieldValue.BlobId == ID.Null)
				return;
			args.Result.FirstPart = wordFieldValue.GetHtmlWithStyles();
		}
	}
}
