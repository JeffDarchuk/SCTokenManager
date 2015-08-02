using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Pipelines.RenderField;

using TokenManager.ContentSearch;
using TokenManager.Data.Interfaces;
using TokenManager.Pipelines;

namespace TokenManager.Management
{

	public class TokenKeeper
	{
		internal static TokenKeeper TokenSingleton;
		public static TokenKeeper CurrentKeeper { get { return TokenSingleton; } }
		public string TokenPrefix { get; set; }
		public string TokenSuffix { get; set; }
		public string Delimiter { get; set; }
		private static readonly ConcurrentDictionary<string, ITokenCollection<IToken>> TokenGroups = new ConcurrentDictionary<string, ITokenCollection<IToken>>();
		private static readonly ConcurrentDictionary<string, List<Tuple<int, int>>> TokenLocations = new ConcurrentDictionary<string, List<Tuple<int, int>>>();
		public TokenKeeper() { }
		public TokenKeeper(string tokenPrefix, string tokenSuffix, string delimiter)
		{
			TokenPrefix = HttpUtility.HtmlEncode(tokenPrefix);
			TokenSuffix = HttpUtility.HtmlEncode(tokenSuffix);
			Delimiter = HttpUtility.HtmlEncode(delimiter);
		}

		public ITokenCollection<IToken> this[string tokenCollection]
		{
			get { return GetTokenCollection<IToken>(tokenCollection); }
			set { LoadTokenGroup(value);}
		}

		/// <summary>
		/// Inserts a token collection into the keeper
		/// </summary>
		/// <param name="collection"></param>
		public virtual void LoadTokenGroup(ITokenCollection<IToken> collection)
		{
			TokenGroups[collection.GetCollectionLabel()] = collection;
		}

		/// <summary>
		/// handles replacing the text from a render field pipeline
		/// </summary>
		/// <param name="args"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public virtual string ReplaceRTETokens(RenderFieldArgs args, string text)
		{
			StringBuilder sb = new StringBuilder(text);
			var current = args.GetField().ID;
			if (!TrackTokens(args.Item.ID, current, text))
				return text;
			foreach (var location in TokenLocations[args.Item.ID.ToString() + current])
			{
				string token = sb.ToString(location.Item1, location.Item2);
				if (token.StartsWith(TokenPrefix) && token.EndsWith(TokenSuffix))
					sb.Replace(token, ParseTokenValueFromTokenIdentifier(token, args.Item), location.Item1, location.Item2);
			}
			return sb.ToString();
		}

		/// <summary>
		/// returns all the tokens in a field
		/// </summary>
		/// <param name="field"></param>
		/// <returns>tokens used in this field</returns>
		public virtual IEnumerable<IToken> ParseTokens(Field field)
		{
			return ParseTokenIdentifiers(field).Select(ParseITokenFromText);
		}

		/// <summary>
		/// from the field it extracts all the token identifying strings
		/// </summary>
		/// <param name="field"></param>
		/// <param name="excludeIdentifierComponenets"></param>
		/// <returns>token identifying strings</returns>
		public virtual IEnumerable<string> ParseTokenIdentifiers(Field field, bool excludeIdentifierComponenets = false)
		{
			string text = field.Value;
			StringBuilder sb = new StringBuilder(text);
			var locations = ParseTokenLocations(field);
			if (locations != null && locations.Any())
				foreach (var location in locations)
				{
					var token = !excludeIdentifierComponenets ? sb.ToString(location.Item1, location.Item2) : sb.ToString(location.Item1+TokenPrefix.Length, location.Item2-TokenSuffix.Length-TokenPrefix.Length).Replace(Delimiter, "");

					yield return token;
				}
		}

		/// <summary>
		/// finds the locations of all the tokens in the field
		/// </summary>
		/// <param name="field"></param>
		/// <returns>list of tuples that have two ints that correspond to the token index, and length of the token</returns>
		public virtual List<Tuple<int, int>> ParseTokenLocations(Field field)
		{
			var text = field.Value;
			if (TrackTokens(field.Item.ID, field.ID, text))
			{
				return TokenLocations[field.Item.ID.ToString() + field.ID];
			}
			return null;
		} 

		/// <summary>
		/// given a token identifying text this returns the IToken object
		/// </summary>
		/// <param name="token"></param>
		/// <returns>IToken for the identifier</returns>
		public virtual IToken ParseITokenFromText(string token)
		{
			return ParseITokenFromText(token, null);
		}

		/// <summary>
		/// given a token identifying text this returns the IToken object with an item for checking context validity
		/// </summary>
		/// <param name="token"></param>
		/// <param name="item"></param>
		/// <returns>IToken for the identifier</returns>
		public virtual IToken ParseITokenFromText(string token, Item item)
		{
			var tokenLen = token.Length - TokenSuffix.Length - TokenPrefix.Length;
			token = token.Substring(TokenPrefix.Length, tokenLen);
			var parts = token.Split(new[] { Delimiter }, StringSplitOptions.None);
			if (TokenGroups.ContainsKey(parts[0]) && TokenGroups[parts[0]].IsCurrentContextValid(item) && parts.Length == 2)
				return TokenGroups[parts[0]].GetToken(parts[1]);
			return null;
		}

		/// <summary>
		/// given a token identifier this returns the value of the token with an optional item for checking context validity
		/// </summary>
		/// <param name="token"></param>
		/// <param name="item"></param>
		/// <returns>token value</returns>
		public virtual string ParseTokenValueFromTokenIdentifier(string token, Item item = null)
		{
			IToken t = ParseITokenFromText(token, item);
			return t != null ? t.Value : string.Empty;
		}

		/// <summary>
		/// based on current token keeper properties it constructs a token identifier
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns>token identifier</returns>
		public virtual string GetTokenIdentifier(string category, string token)
		{
			return TokenPrefix + category + Delimiter + token + TokenSuffix;
		}

		/// <summary>
		/// gets the value of the token belonging to the specific category
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public virtual string GetTokenValue(string category, string token)
		{
			if (TokenGroups.ContainsKey(category) && TokenGroups[category].IsCurrentContextValid())
				return TokenGroups[category][token];
			return null;
		}

		/// <summary>
		/// returns the token value of the first 
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public virtual string GetTokenValue(string token)
		{
			var tokenCollection = TokenGroups.Values.FirstOrDefault(t => t.HasToken(token));
			return tokenCollection!= null ? tokenCollection[token] : null;
		}

		/// <summary>
		/// finds everywhere a specified token is used
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns>enumerable of all occurances of this token</returns>
		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token)
		{
			return GetTokenOccurances(category, token, "master");
		}

		/// <summary>
		/// finds everywhere a specified token is used in a specific database
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <param name="db"></param>
		/// <returns>enumerable of all occurances of this token</returns>
		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, Database db)
		{
			return GetTokenOccurances(category, token, db.Name);
		}

		/// <summary>
		/// finds everywhere a specified token is used in a specific database
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <param name="db"></param>
		/// <returns>enumerable of all occurances of this token</returns>
		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, string db)
		{
			var index = ContentSearchManager.GetIndex("sitecore_" + db + "_index")
				.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck);
			var query = index.GetQueryable<ContentSearchTokens>()
				.Where(t => t.Tokens.Contains(category + token));
			return query;
		} 

		/// <summary>
		/// gets all the labels for the token collections
		/// </summary>
		/// <returns>token collection labels</returns>
		public virtual IEnumerable<string> GetTokenCollectionNames()
		{
			return TokenGroups.Keys.Where(s=>TokenGroups[s].IsCurrentContextValid());
		}

		/// <summary>
		/// gets all the token collections
		/// </summary>
		/// <returns>token collections</returns>
		public virtual IEnumerable<ITokenCollection<IToken>> GetTokenCollections()
		{
			return TokenGroups.Values.Where(c=>c.IsCurrentContextValid());
		} 

		/// <summary>
		/// gets a specific token collection by name
		/// </summary>
		/// <param name="collectionName"></param>
		/// <returns>token collection</returns>
		public virtual ITokenCollection<T> GetTokenCollection<T>(string collectionName)
			where T : IToken
		{
			if (TokenGroups.ContainsKey(collectionName) && TokenGroups[collectionName].IsCurrentContextValid())
				return TokenGroups[collectionName] as ITokenCollection<T>;
			return null;
		}

		/// <summary>
		/// gets all token names under a specific category
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetTokens(string category)
		{
			var collection = GetTokenCollection<IToken>(category);
			if (collection != null)
				return collection.GetTokens();
			return null;
		}

		/// <summary>
		/// gets token object from category and token names
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		/// <returns>token object</returns>
		public virtual IToken GetToken(string category, string token)
		{
			var collection = GetTokenCollection<IToken>(category);
			if (collection != null)
				return collection.GetToken(token);
			return null;
		}

		/// <summary>
		/// removes to the token collection from the keeper
		/// </summary>
		/// <param name="collectionLabel"></param>
		/// <returns>the collection removed</returns>
		public virtual ITokenCollection<IToken> RemoveGroup(string collectionLabel)
		{
			ITokenCollection<IToken> ret;
			TokenGroups.TryRemove(collectionLabel, out ret);
			return ret;
		}
		
		/// <summary>
		/// clears the token location caches for the specific item and field
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="fieldId"></param>
		public virtual void ResetTokenLocations(ID itemId, ID fieldId)
		{
			List<Tuple<int,int>> ignored;
			string key = itemId.ToString() + fieldId;
			if (TokenLocations.ContainsKey(key))
				TokenLocations.TryRemove(key, out ignored);
		}

		/// <summary>
		/// given an item that represents a token collection it will return the collection
		/// </summary>
		/// <param name="item"></param>
		/// <returns>token collection</returns>
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

		/// <summary>
		/// finds the token locations and stores it in a cache
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="fieldId"></param>
		/// <param name="text"></param>
		/// <returns>if there are any tokens found</returns>
		private bool IdentifyTokenLocations(ID itemId, ID fieldId, string text)
		{
			if (TokenPrefix == null || Delimiter == null || TokenSuffix == null)
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
			TokenLocations.AddOrUpdate(itemId + fieldId.ToString(), locations, (key, value) => locations);
			return locations.Any();
		}

		/// <summary>
		/// finds if the locations are cached and if not calls to have them identified
		/// </summary>
		/// <param name="itemId"></param>
		/// <param name="fieldId"></param>
		/// <param name="text"></param>
		/// <returns>if there are any tokens</returns>
		private bool TrackTokens(ID itemId, ID fieldId, string text)
		{
			var key = itemId + fieldId.ToString();
			if (!TokenLocations.ContainsKey(key))
				if (!IdentifyTokenLocations(itemId, fieldId, text))
					return false;
			if (!TokenLocations[key].Any())
				return false;
			return true;
		}
	}
}
