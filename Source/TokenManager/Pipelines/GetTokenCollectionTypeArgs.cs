using Sitecore.Data.Items;
using Sitecore.Pipelines;

using TokenManager.Data.Interfaces;

namespace TokenManager.Pipelines
{

	public class GetTokenCollectionTypeArgs : PipelineArgs
	{
		public Item GroupItem { get; set; }
		public ITokenCollection Collection { get; set; }
	}
}
