using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Sitecore.Data;
using Sitecore.Data.Fields;

using TokenManager.ContentSearch;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Handlers
{

	/// <summary>
	/// collects various stats on token usage
	/// </summary>
	class TokenStats
	{
		private const string _contentFolderGuid = "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}";
		private string _category;
		private string _token;
		private Database _database;

		/// <summary>
		/// set up stats generator with the specified token identifier
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		public TokenStats(string category, string token)
		{
			_category = category;
			_token = token;
			_database = Database.GetDatabase("master");
			
		}

		/// <summary>
		/// gathers stats for the current token
		/// </summary>
		/// <returns>dynamic object containing stats for the token</returns>
		public dynamic GetStats()
		{
			dynamic ret = new ExpandoObject();
			ret.Uses = 0;
			ret.TokenCategory = _category;
			ret.Token = _token;
			ret.TokenValue = TokenKeeper.CurrentKeeper.GetTokenValue(_category, _token);
			ret.ByItem = new Dictionary<ID, ExpandoObject>();
			ret.TokenCollectionItemId = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(_category).GetBackingItemId();
			ret.TokenItemId = TokenKeeper.CurrentKeeper.GetToken(_category, _token).GetBackingItemId();
			CrunchStats(ret);
			return ret;
		}

		/// <summary>
		/// traverses all instances of the token
		/// </summary>
		/// <param name="ret"></param>
		public void CrunchStats(dynamic ret)
		{
			foreach (ContentSearchTokens currentItem in TokenKeeper.CurrentKeeper.GetTokenOccurances(_category,_token))
			{
				var item = _database.GetItem(ID.Parse(currentItem.Id));
				foreach (var field in item.Fields.Where(f => f.Type == "Rich Text"))
					ProcessThisToken(currentItem, field, ret);
			}
		}
		
		/// <summary>
		/// gathers information about the token usage on a particular field
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="f"></param>
		/// <param name="ret"></param>
		private void ProcessThisToken(ContentSearchTokens tokens, Field f, dynamic ret)
		{
			var tokenCount = tokens.Tokens.Count(t => t == _category + _token);
			if (tokenCount == 0) return;
			ret.Uses+= tokenCount;
			if (!ret.ByItem.ContainsKey(f.Item.ID))
			{
				dynamic cur = new ExpandoObject();
				cur.Count = 0;
				cur.DisplayName = f.Item.DisplayName;
				cur.Path = f.Item.Paths.FullPath;
				cur.ID = f.Item.ID;
				cur.FieldName = f.Name;
				ret.ByItem[f.Item.ID] = cur;
			}
					
			ret.ByItem[f.Item.ID].Count+=tokenCount;
			
		}
	}
}
