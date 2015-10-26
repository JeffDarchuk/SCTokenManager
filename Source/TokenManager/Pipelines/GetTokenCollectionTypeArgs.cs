using Sitecore.Data.Items;
using Sitecore.Pipelines;
using TokenManager.Data.Interfaces;

namespace TokenManager.Pipelines
{

	public class GetTokenCollectionTypeArgs : PipelineArgs
	{
		public Item CollectionItem { get; set; }
		public ITokenCollection<IToken> Collection { get; set; }
	}
}
