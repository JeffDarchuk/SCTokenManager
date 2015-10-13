using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using TokenManager.ContentSearch;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Handlers.TokenOperations
{

	/// <summary>
	/// collects various stats on token usage
	/// </summary>
	class TokenStats
	{
		private readonly string _category;
		private readonly string _token;
		private readonly Database _database;

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
		    try
		    {
		        ret.TokenValue = TokenKeeper.CurrentKeeper.GetTokenValue(_category, _token, null);
		    }
		    catch (Exception e)
		    {
		        ret.TokenValue = "[Couldn't get token value without context data]";
		    }
		    ret.ByItem = new Dictionary<string, ExpandoObject>();
			ret.TokenCollectionItemId = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(_category).GetBackingItemId();
			ret.TokenItemId = TokenKeeper.CurrentKeeper.GetToken(_category, _token).GetBackingItemId();
			CrunchStats(ret);
		    ret.ByItem = ret.ByItem.Values;
			return ret;
		}

		/// <summary>
		/// traverses all instances of the token
		/// </summary>
		/// <param name="ret"></param>
		public void CrunchStats(dynamic ret)
		{
			foreach (ContentSearchTokens current in TokenKeeper.CurrentKeeper.GetTokenOccurances(_category,_token))
			{
                Item item = _database.GetItem(ID.Parse(current.Id), !string.IsNullOrWhiteSpace(current.Language) && LanguageManager.IsValidLanguageName(current.Language) ? LanguageManager.GetLanguage(current.Language) : LanguageManager.DefaultLanguage);
                foreach (var field in item.Fields.Where(f => f.Type == "Rich Text"))
					ProcessThisToken(field, ret);
			}
		}

		/// <summary>
		/// gathers information about the token usage on a particular field
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="f"></param>
		/// <param name="ret"></param>
		private void ProcessThisToken(Field f, dynamic ret)
		{
		    var tokenCount =
		        TokenKeeper.CurrentKeeper.ParseTokenIdentifiers(f)
		            .Select(TokenKeeper.CurrentKeeper.TokenProperties)
		            .Count(p => p["Category"] == _category && p["Token"] == _token);
			if (tokenCount == 0) return;
			ret.Uses+= tokenCount;
			if (!ret.ByItem.ContainsKey(f.Item.ID+f.Language.Name))
			{
				dynamic cur = new ExpandoObject();
				cur.Count = 0;
				cur.DisplayName = f.Item.DisplayName;
				cur.Path = f.Item.Paths.FullPath;
				cur.ID = f.Item.ID;
				cur.FieldName = f.Name;
			    cur.Language = f.Language.Name;
                ret.ByItem[f.Item.ID + f.Language.Name] = cur;
			}

            ret.ByItem[f.Item.ID + f.Language.Name].Count += tokenCount;
			
		}
	}
}
