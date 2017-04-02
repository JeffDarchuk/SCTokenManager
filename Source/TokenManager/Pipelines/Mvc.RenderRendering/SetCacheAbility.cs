using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using TokenManager.Management;

namespace TokenManager.Pipelines.Mvc.RenderRendering
{
	public class SetCacheAbility : SetCacheability
	{
		protected override bool IsCacheable(Rendering rendering, RenderRenderingArgs args)
		{
			bool ret = base.IsCacheable(rendering, args);
			if (ret == false)
				return false;
			return !(args.PageContext.Database.GetItem(rendering.DataSource)?.HasTokens() ?? false);
		}
	}
}
