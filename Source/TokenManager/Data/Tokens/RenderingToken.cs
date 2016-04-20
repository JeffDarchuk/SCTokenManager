using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Helpers;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{
	public class RenderingToken : SitecoreBasedToken
	{

		public RenderingToken(string name, ID backingId) : base(name, backingId)
		{
		}
		public override IEnumerable<ITokenData> ExtraData()
		{
			yield return new IdTokenData("Datasource", "Datasource for the rendering", true);
		}

		public override string Value(NameValueCollection extraData)
		{
			InternalLinkField renderingItem = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(BackingId).Fields["Rendering"];
			TextWriter tw = new StringWriter();
			var h = new HtmlHelper(new ViewContext(new ControllerContext(), new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), tw), new ViewPage());
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(extraData["Datasource for the rendering"]))
				sb.Append(new SitecoreHelper(h).Rendering(renderingItem.TargetID.ToString(),
					new { Datasource = extraData["Datasource for the rendering"] }));
			return sb.ToString();
		}

		public class FakeView : IView
		{
			public void Render(ViewContext viewContext, TextWriter writer)
			{
				throw new InvalidOperationException();
			}
		}


	}

}
