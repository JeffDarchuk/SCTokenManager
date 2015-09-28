using System.Collections.Generic;
using Sitecore.ContentSearch;

namespace TokenManager.ContentSearch
{
	public class ContentSearchTokens
	{
		[IndexField("_tokens")] 
		public List<string> Tokens {get; set; }
		[IndexField("_group")]
		public string Id { get; set; }
        [IndexField("_fullpath")]
        public string Path { get; set; }
        [IndexField("_language")]
        public string Language { get; set; }
	}
}
