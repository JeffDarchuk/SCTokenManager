using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using TokenManager.Management;

namespace TokenManager.ContentSearch
{

	internal class Tokens : IComputedIndexField
	{
		/// <summary>
		/// method for the custom indexed field for tracking tokens
		/// </summary>
		/// <param name="indexable"></param>
		/// <returns>list of tokens used in the item</returns>
		public object ComputeFieldValue(IIndexable indexable)
		{
			return GetTokens(indexable as SitecoreIndexableItem);
		}

		/// <summary>
		/// gets all the token identifiers in the item
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public List<string> GetTokens(Item item)
		{
            if (!item.IsTokenManagerItem())
			    return item.Fields.Where(f => f.Type == "Rich Text").SelectMany(GetAllTokenUniqueIds).ToList();
		    return null;
		}

		public string FieldName { get; set; }

		public string ReturnType { get; set; }

	    private IEnumerable<string> GetAllTokenUniqueIds(Field f)
	    {
	        return
	            TokenKeeper.CurrentKeeper.ParseTokenIdentifiers(f)
	                .Select(TokenKeeper.CurrentKeeper.TokenProperties)
	                .Select(t => t["Category"] + t["Token"]);
	    }
	}
}
