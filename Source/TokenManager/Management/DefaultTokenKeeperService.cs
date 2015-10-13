using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using Sitecore.Pipelines.RenderField;
using TokenManager.ContentSearch;
using TokenManager.Data.Interfaces;
using TokenManager.Pipelines;

namespace TokenManager.Management
{
    class DefaultTokenKeeperService : ITokenKeeperService
    {
        public string TokenPrefix { get { return "<a class=\"token-manager-token\" href=\"/TokenManager?"; } }
		public string TokenSuffix { get { return "</a>"; }}
        public string TokenCss { get; set; }
		private static readonly ConcurrentDictionary<string, ITokenCollection<IToken>> TokenGroups = new ConcurrentDictionary<string, ITokenCollection<IToken>>();
		private static readonly ConcurrentDictionary<string, List<Tuple<int, int>>> TokenLocations = new ConcurrentDictionary<string, List<Tuple<int, int>>>();

	    public DefaultTokenKeeperService()
	    {
	    }

        public DefaultTokenKeeperService(string tokenCss)
	    {
            TokenCss = tokenCss;
	    }

	    public ITokenCollection<IToken> this[string tokenCollection]
		{
			get { return GetTokenCollection<IToken>(tokenCollection); }
			set { LoadTokenGroup(value);}
		}

		public virtual void LoadTokenGroup(ITokenCollection<IToken> collection)
		{
			if (collection != null)
				TokenGroups[collection.GetCollectionLabel()] = collection;
		}

		public virtual string ReplaceRTETokens(RenderFieldArgs args, string text)
		{
			StringBuilder sb = new StringBuilder(text);
			var current = args.GetField().ID;
			if (!TrackTokens(args.Item.ID, current, args.Item.Language, args.Item.Version.Number, text))
				return text;
			foreach (var location in TokenLocations[args.Item.ID.ToString() + current+args.Item.Language.Name+args.Item.Version.Number])
			{
			    if (location.Item1 + location.Item2 > text.Length)
			    {
			        ResetTokenLocations(args.Item.ID,current, args.Item.Language, args.Item.Version.Number);
			        return ReplaceRTETokens(args, text);
			    }
				string token = sb.ToString(location.Item1, location.Item2);
				if (token.StartsWith(TokenPrefix) && token.EndsWith(TokenSuffix))
					sb.Replace(token, ParseTokenValueFromTokenIdentifier(token, args.Item), location.Item1, location.Item2);
				else
				{
                    ResetTokenLocations(args.Item.ID, current, args.Item.Language, args.Item.Version.Number);
				    return ReplaceRTETokens(args, text);
				}
			}
			return sb.ToString();
		}

		public virtual IEnumerable<IToken> ParseTokens(Field field)
		{
			return ParseTokenIdentifiers(field).Select(ParseITokenFromText);
		}

		public virtual IEnumerable<string> ParseTokenIdentifiers(Field field)
		{
			string text = field.Value;
			StringBuilder sb = new StringBuilder(text);
			var locations = ParseTokenLocations(field);
            List<string> ret = new List<string>();
			if (locations != null && locations.Any())
				foreach (var location in locations)
				{
				    var tokenProps = sb.ToString(location.Item1, location.Item2);
				    if (!tokenProps.StartsWith(TokenPrefix) || !tokenProps.EndsWith(TokenSuffix))
				    {
				        ResetTokenLocations(field.Item.ID, field.ID, field.Language, field.Item.Version.Number);
                        return ParseTokenIdentifiers(field);
				    }
					ret.Add(tokenProps);
				}
		    return ret;
		}

		public virtual List<Tuple<int, int>> ParseTokenLocations(Field field)
		{
			var text = field.Value;
			if (TrackTokens(field.Item.ID, field.ID, field.Language, field.Item.Version.Number, text))
			{
				return TokenLocations[field.Item.ID.ToString() + field.ID+field.Language.Name+field.Item.Version.Number];
			}
			return new List<Tuple<int, int>>();
		} 

		public virtual IToken ParseITokenFromText(string token)
		{
		    var props = TokenProperties(token);
			return ParseITokenFromProps(props);
		}

		public virtual IToken ParseITokenFromProps(NameValueCollection props)
		{
		    return GetToken(props["Category"], props["Token"]);
		}

		public virtual string ParseTokenValueFromTokenIdentifier(string token, Item item = null)
		{
		    var props = TokenProperties(token);
            IToken t = ParseITokenFromProps(props);
			return t != null ? t.Value(props) : string.Empty;
		}

	    public virtual string GetTokenIdentifier(NameValueCollection data)
	    {
	        return GetTokenIdentifier(data["Category"],data["Token"], data.AllKeys.Where(k=>k != "Category" && k != "Token").ToDictionary(k => k, k => data[k]));
	    }

		public virtual string GetTokenIdentifier(string category, string token, dynamic data)
		{
		    return GetTokenIdentifier(category, token, data as IDictionary<string, object>);
		}

	    public virtual string GetTokenIdentifier(string category, string token, IDictionary<string, object> fields)
	    {
	        var ret = HttpUtility.ParseQueryString("");
            ret["Category"]= category;
            ret["Token"] = token;
            if (fields != null)
                foreach (string key in fields.Keys)
                    ret.Add(key, fields[key].ToString());
	        return string.Format("{0}{1}\" {5}>{2} > {3}{4}", TokenPrefix, ret, category,token, TokenSuffix, string.Format("style='{0}'", TokenCss));
	    }

		public virtual string GetTokenValue(string category, string token, NameValueCollection extraData)
		{
			if (TokenGroups.ContainsKey(category) && TokenGroups[category].IsCurrentContextValid())
				return TokenGroups[category][token].Value(extraData);
			return null;
		}

		public virtual string GetTokenValue(string token, NameValueCollection extraData)
		{
			var tokenCollection = TokenGroups.Values.FirstOrDefault(t => t.HasToken(token));
			return tokenCollection!= null ? tokenCollection[token].Value(extraData) : null;
		}

		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token)
		{
			return GetTokenOccurances(category, token, "master");
		}
        public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, ID root)
        {
            return GetTokenOccurances(category, token, Database.GetDatabase("master"), root);
        }

		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, Database db)
		{
            return GetTokenOccurances(category, token, db.Name);
		}
        public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, Database db, ID root)
        {
            var item = db.GetItem(root);
            if (item == null)
                return GetTokenOccurances(category, token, db.Name);
            else
            {
                var tmp = GetTokenOccurances(category, token, db.Name).ToList();
                return tmp.Where(x => x.Path.StartsWith(item.Paths.FullPath));
            }
        }

	    public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, string db)
		{
		    
		    var index = ContentSearchManager.GetIndex("sitecore_" + db + "_index")
		        .CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck);
		    var query = index.GetQueryable<ContentSearchTokens>()
		        .Where(t => t.Tokens.Contains(category + token));
            return query;
		    return null;
		} 

		public virtual IEnumerable<string> GetTokenCollectionNames()
		{
			return TokenGroups.Keys.Where(s=>TokenGroups[s].IsCurrentContextValid());
		}

		public virtual IEnumerable<ITokenCollection<IToken>> GetTokenCollections()
		{
			return TokenGroups.Values.Where(c=>c.IsCurrentContextValid());
		} 

		public virtual ITokenCollection<T> GetTokenCollection<T>(string collectionName)
			where T : IToken
		{
			if (TokenGroups.ContainsKey(collectionName) && TokenGroups[collectionName].IsCurrentContextValid())
				return TokenGroups[collectionName] as ITokenCollection<T>;
			return null;
		}

        public ITokenCollection<T> GetTokenCollection<T>(ID backingItemId) where T : IToken
        {
            return GetTokenCollections().FirstOrDefault(x => x.GetBackingItemId() == backingItemId) as ITokenCollection<T>;
        }

        public virtual IEnumerable<IToken> GetTokens(string category)
		{
			var collection = GetTokenCollection<IToken>(category);
			if (collection != null)
				return collection.GetTokens();
			return null;
		}

		public virtual IToken GetToken(string category, string token)
		{
			var collection = GetTokenCollection<IToken>(category);

		    if (collection != null)
		    {
                IToken ret = collection.GetToken(token);
		        return ret;
		    }
		    return null;
		}

		public virtual ITokenCollection<IToken> RemoveGroup(string collectionLabel)
		{
			ITokenCollection<IToken> ret;
			TokenGroups.TryRemove(collectionLabel, out ret);
			return ret;
		}
		
		public virtual void ResetTokenLocations(ID itemId, ID fieldId, Language language, int versionNumber)
		{
			List<Tuple<int,int>> ignored;
			string key = itemId.ToString() + fieldId + language.Name+versionNumber;
			if (TokenLocations.ContainsKey(key))
				TokenLocations.TryRemove(key, out ignored);
		}

		public virtual ITokenCollection<IToken> GetCollectionFromItem(Item item)
		{
			var args = new GetTokenCollectionTypeArgs()
			{
				GroupItem = item
			};
			var pipeline = CorePipelineFactory.GetPipeline("getTokenGroup", string.Empty);
			pipeline.Run(args);
			return args.Collection;
		}

        public virtual NameValueCollection TokenProperties(string tokenIdentifier)
        {
            if (string.IsNullOrWhiteSpace(tokenIdentifier))
                return new NameValueCollection();
            if (tokenIdentifier.StartsWith(TokenPrefix) && tokenIdentifier.EndsWith(TokenSuffix))
            {
                int end = tokenIdentifier.IndexOf('"', TokenPrefix.Length);
                tokenIdentifier = HttpUtility.HtmlDecode(tokenIdentifier.Substring(TokenPrefix.Length,
                    end - TokenPrefix.Length));
            }
            return HttpUtility.ParseQueryString(tokenIdentifier);
        }

        public virtual bool IsInToken(Field field, int startIndex, int length)
        {
            var locations = ParseTokenLocations(field);
            if (locations == null)
                return false;
            foreach (var location in locations)
            {
                if (location.Item1+location.Item2 < startIndex)
                    break;
                if (startIndex < location.Item1 && startIndex + length > location.Item1)
                    return true;
                if (startIndex + length > location.Item1 + location.Item2 && startIndex < location.Item1 + location.Item2)
                    return true;
                if (startIndex > location.Item1 && startIndex + length < location.Item1 + location.Item2)
                    return true;
            }
            return false;
        }

        public virtual Database GetDatabase()
        {
            return Context.ContentDatabase ?? Context.Database ?? Database.GetDatabase("master");
        }

		/// <summary>
		/// finds the token locations and stores it in a cache
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="fieldId"></param>
		/// <param name="text"></param>
		/// <returns>if there are any tokens found</returns>
		private bool IdentifyTokenLocations(ID itemId, ID fieldId, Language language, int version, string text)
		{
			if (TokenPrefix == null || TokenSuffix == null)
				return false;
			List<Tuple<int, int>> locations = new List<Tuple<int, int>>();
			int startIndex = text.IndexOf(TokenPrefix);
			var endIndex = -1;
			while (startIndex > -1)
			{
				endIndex = text.IndexOf(TokenSuffix, startIndex);
				if (endIndex != -1)
					endIndex += TokenSuffix.Length;
				else
					break;
				locations.Insert(0, new Tuple<int, int>(startIndex, endIndex - startIndex));
				startIndex = text.IndexOf(TokenPrefix, endIndex);
			}
			TokenLocations.AddOrUpdate(itemId + fieldId.ToString()+language.Name+version, locations, (key, value) => locations);
			return locations.Any();
		}

		/// <summary>
		/// finds if the locations are cached and if not calls to have them identified
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="fieldId"></param>
		/// <param name="text"></param>
		/// <returns>if there are any tokens</returns>
		private bool TrackTokens(ID itemId, ID fieldId, Language language, int version, string text)
		{
			var key = itemId + fieldId.ToString()+ language.Name+version;
			if (!TokenLocations.ContainsKey(key))
				if (!IdentifyTokenLocations(itemId, fieldId, language, version, text))
					return false;
			if (!TokenLocations[key].Any())
				return false;
			return true;
		}


    }
}
