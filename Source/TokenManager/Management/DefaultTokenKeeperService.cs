using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using Sitecore.Pipelines.RenderField;
using Sitecore.SecurityModel;
using TokenManager.Collections;
using TokenManager.ContentSearch;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;
using TokenManager.Data.TokenExtensions;
using TokenManager.Data.Tokens;
using TokenManager.Pipelines;

namespace TokenManager.Management
{
	class DefaultTokenKeeperService : ITokenKeeperService
	{
		public string TokenPrefix => "<code contenteditable=\"false\" unselectable=\"on\" class=\"token-manager-token\" href=\"/TokenManager?";
		public string TokenSuffix => "</code>";
		public string TokenRegex = "<[^>]*?/TokenManager?.*?>";
		public string TokenCss { get; set; }
		private static readonly ConcurrentDictionary<string, ITokenCollection<IToken>> TokenCollections = new ConcurrentDictionary<string, ITokenCollection<IToken>>();
		private static readonly ConcurrentDictionary<string, DateTime> TokenCacheUpdateTimes = new ConcurrentDictionary<string, DateTime>();
		private static readonly ConcurrentDictionary<string, Tuple<DateTime, List<Tuple<int, int>>>> TokenLocations = new ConcurrentDictionary<string, Tuple<DateTime, List<Tuple<int, int>>>>();

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
			set { LoadTokenCollection(value); }
		}

		public virtual void LoadTokenCollection(ITokenCollection<IToken> collection)
		{
			if (collection != null)
			{
				TokenCacheUpdateTimes[collection.GetCollectionLabel()] =
					GetDatabase().GetItem(collection.GetBackingItemId()).Statistics.Updated;
				TokenCollections[collection.GetCollectionLabel()] = collection;
			}
		}

		public void LoadAutoToken(AutoToken token)
		{
			using (new SecurityDisabler())
			{
				if (TokenCollections.ContainsKey(token.CollectionName))
				{
					TokenCollections[token.CollectionName].AddOrUpdateToken(token.Token, token);
				}
				else
				{
					TokenCollections[token.CollectionName] = new AutoTokenCollection(token);
					TokenCollections[token.CollectionName].AddOrUpdateToken(token.Token, token);
				}
				var db = Factory.GetDatabase("core", false);
				if (db == null) return;

				TokenButton tb = token.TokenButton();

				ID buttonId = GuidUtility.GetId("tokenmanager", $"{token.CollectionName}{token.Token}");

				Item button = db.DataManager.DataEngine.GetItem(buttonId, LanguageManager.DefaultLanguage, Sitecore.Data.Version.Latest);

				if (tb != null)
				{
					if (button != null)
					{
						if (
							button["Click"] == $"TokenSelector{Regex.Replace(token.CollectionName, "[^A-Za-z0-9_]", "")}{Regex.Replace(token.Token, "[^A-Za-z0-9_]", "")}" &&
							button[FieldIDs.DisplayName] == tb.Name &&
							button["Shortcut"] == $"?Category={token.CollectionName}&Token={token.Token}" &&
							button[FieldIDs.Sortorder] == tb.SortOrder.ToString() &&
							button[FieldIDs.Icon] == (string.IsNullOrWhiteSpace(tb.Icon) ? token.TokenIcon : tb.Icon))
						{
							return;
						}
					}
					else
					{
						Item parent = db.DataManager.DataEngine.GetItem(new ID(Constants.Core.RteParent), LanguageManager.DefaultLanguage, Sitecore.Data.Version.Latest);

						if (parent == null) return;

						button = db.DataManager.DataEngine.CreateItem(tb.Name, parent, new ID(Constants.Core.ButtonTemplate), buttonId);
					}

					button.Editing.BeginEdit();
					button["Click"] = $"TokenSelector{Regex.Replace(token.CollectionName, "[^A-Za-z0-9_]", "")}{Regex.Replace(token.Token, "[^A-Za-z0-9_]", "")}";
					button[FieldIDs.DisplayName] = tb.Name;
					button["Shortcut"] = $"?Category={token.CollectionName}&Token={token.Token}";
					button[FieldIDs.Sortorder] = tb.SortOrder.ToString();
					button[FieldIDs.Icon] = string.IsNullOrWhiteSpace(tb.Icon) ? token.TokenIcon : tb.Icon;
					button.Editing.EndEdit(false, true);
					button.Database.Caches.ItemCache.RemoveItem(buttonId);
					button.Database.Caches.DataCache.RemoveItemInformation(buttonId);
				}
				else if (button != null)
				{
					button.Recycle();
				}
			}
		}

		public virtual string ReplaceRTETokens(RenderFieldArgs args, string text)
		{
			StringBuilder sb = new StringBuilder(text);

			var current = args.GetField().ID;
			if (!TrackTokens(args.Item, current, args.Item.Language, args.Item.Version.Number, text))
				return text;
			foreach (
				var location in
					TokenLocations[args.Item.ID.ToString() + current + args.Item.Language.Name + args.Item.Version.Number].Item2)
			{
				if (location.Item1 + location.Item2 > text.Length && !args.WebEditParameters.ContainsKey("reseted"))
				{
					ResetTokenLocations(args.Item.ID, current, args.Item.Language, args.Item.Version.Number);
					args.WebEditParameters["reseted"] = "1";
					return ReplaceRTETokens(args, text);
				}
				try
				{
					string token = sb.ToString(location.Item1, location.Item2);
					if (Regex.IsMatch(token, TokenRegex))
					{
						try
						{
							var value = ParseTokenValueFromTokenIdentifier(token, args.Item);
							sb.Replace(token, value, location.Item1, location.Item2);
						}
						catch
						{
							// error rendering token - still replace the raw value, but do it with an empty string
							sb.Replace(token, string.Empty, location.Item1, location.Item2);
							throw;
						}
					}
					else if (!args.WebEditParameters.ContainsKey("reseted"))
					{
						ResetTokenLocations(args.Item.ID, current, args.Item.Language, args.Item.Version.Number);
						args.WebEditParameters["reseted"] = "1";
						return ReplaceRTETokens(args, text);
					}
				}
				catch (Exception e)
				{
					Log.Error("unable to expand tokens for item " + args.Item.ID, e, this);
				}
			}
			return sb.ToString();
		}

		public virtual IEnumerable<IToken> ParseTokens(Field field, Item item = null)
		{
			return ParseTokenIdentifiers(field).Select(x => ParseITokenFromText(x, item));
		}

		public virtual IEnumerable<string> ParseTokenIdentifiers(Field field)
		{
			string text = field.Value;
			StringBuilder sb = new StringBuilder(text);
			var locations = ParseTokenLocations(field);
			List<string> ret = new List<string>();
			if (locations == null) return ret;
			try
			{
				foreach (var tokenProps in locations.Item2.Select(location => sb.ToString(location.Item1, location.Item2)))
				{
					if (!Regex.IsMatch(tokenProps, TokenRegex))
					{
						ResetTokenLocations(field.Item.ID, field.ID, field.Language, field.Item.Version.Number);
						return ParseTokenIdentifiers(field);
					}
					ret.Add(tokenProps);
				}
			}
			catch (ArgumentOutOfRangeException) //our location cache is invalid, resetting
			{
				ResetTokenLocations(field.Item.ID, field.ID, field.Language, field.Item.Version.Number);
				return ParseTokenIdentifiers(field);
			}
			return ret;
		}

		public virtual Tuple<DateTime, List<Tuple<int, int>>> ParseTokenLocations(Field field)
		{
			var text = field.Value;
			return TrackTokens(field.Item, field.ID, field.Language, field.Item.Version.Number, text) ? TokenLocations[field.Item.ID.ToString() + field.ID + field.Language.Name + field.Item.Version.Number] : new Tuple<DateTime, List<Tuple<int, int>>>(DateTime.Now, new List<Tuple<int, int>>());
		}

		public virtual IToken ParseITokenFromText(string token, Item item = null)
		{
			var props = TokenProperties(token);
			return ParseITokenFromProps(props, item);
		}

		public virtual IToken ParseITokenFromProps(TokenDataCollection props, Item item = null)
		{
			return GetToken(props["Category"], props["Token"], item);
		}

		public virtual string ParseTokenValueFromTokenIdentifier(string token, Item item = null)
		{
			var props = TokenProperties(token);
			IToken tokenObject = ParseITokenFromProps(props, item);
			return tokenObject != null ? tokenObject.Value(props) : string.Empty;
		}

		public virtual string GetTokenIdentifier(TokenDataCollection data)
		{
			return GetTokenIdentifier(data["Category"], data["Token"], data.AllKeys.Where(k => k != "Category" && k != "Token").ToDictionary(k => k, k => data[k]));
		}

		public virtual string GetTokenIdentifier(string category, string token, dynamic data)
		{
			return GetTokenIdentifier(category, token, data as IDictionary<string, object>);
		}

		public virtual string GetTokenIdentifier(string category, string token, IDictionary<string, object> fields)
		{
			List<ID> ids = new IdList();
			var ret = new TokenDataCollection();

			ret["Category"] = category;
			ret["Token"] = token;

			IToken itoken = GetToken(category, token);

			if (fields != null)
			{
				foreach (string key in fields.Keys.Where(x => x != "Category" && x != "Token"))
				{
					var grouped = (IDictionary<string, object>) (fields[key] as IDictionary<string, object>)?["grouped"];
					if (grouped == null)
					{
						var value = fields[key]?.ToString() ?? "";
						ret[key] = value;
						ID tmp;
						if (ID.TryParse(value, out tmp))
						{
							ids.Add(tmp);
						}
					}
					else
					{
						StringBuilder sb = new StringBuilder();
						foreach (KeyValuePair<string, object> group in grouped)
						{
							sb.Append(HttpUtility.HtmlEncode(group.Key));
							sb.Append("|=|");
							sb.Append(HttpUtility.HtmlEncode(group.Value.ToString()));
							sb.Append("|||");
							ID tmp;
							if (ID.TryParse(group.Value, out tmp))
							{
								ids.Add(tmp);
							}
						}
						if (sb.Length > 3)
							ret[key] = sb.ToString(0, sb.Length - 3);
					}
				}
			}

			return string.Format("{0}{1}\" {4}>{2}<span style='display:none;'>{5}</span>{3}", TokenPrefix, ret, itoken.TokenIdentifierText(ret).Replace("\n", "").Replace("\r", ""), TokenSuffix, $"style='{itoken.TokenIdentifierStyle(ret)}'", GenerateScLinks(ids));
		}

		private string GenerateScLinks(List<ID> ids)
		{
			StringBuilder sb = new StringBuilder();
			foreach (ID id in ids)
			{
				sb.Append($"<a href=\"~/link.aspx?_id={id.ToShortID().ToString().ToUpper()}&amp;_z=z\">link</a>");
			}
			return sb.ToString();
		}
		public virtual string GetTokenValue(string category, string token, TokenDataCollection extraData)
		{
			if (TokenCollections.ContainsKey(category) && TokenCollections[category].IsCurrentContextValid() && IsCollectionValid(TokenCollections[category]))
				return TokenCollections[category][token].Value(extraData);
			return null;
		}

		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token)
		{
			return GetTokenOccurances(category, token, GetDatabase().Name);
		}
		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, ID root)
		{
			return GetTokenOccurances(category, token, GetDatabase(), root);
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
				return tmp.Where(x => x.Path.StartsWith(item.Paths.FullPath, StringComparison.CurrentCultureIgnoreCase));
			}
		}

		public virtual IEnumerable<ContentSearchTokens> GetTokenOccurances(string category, string token, string db)
		{
			var index = ContentSearchManager.GetIndex("sitecore_" + db + "_index")
				.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck);
			var query = index.GetQueryable<ContentSearchTokens>()
				.Where(t => t.Tokens.Contains(category + token));
			return query;
		}

		public virtual IEnumerable<string> GetTokenCollectionNames()
		{
			return GetTokenCollections().Select(x => x.GetCollectionLabel());
		}

		public virtual IEnumerable<ITokenCollection<IToken>> GetTokenCollections()
		{
			return TokenCollections.Values.Where(c => c.IsCurrentContextValid()).OrderBy(x => x.GetCollectionLabel());
		}

		public virtual ITokenCollection<T> GetTokenCollection<T>(string collectionName, Item item = null)
			where T : IToken
		{

			if (!TokenCollections.ContainsKey(collectionName) || !IsCollectionValid(TokenCollections[collectionName]))
			{
				RefreshTokenCollection(collectionName);
				var collection = TokenCollections.ContainsKey(collectionName) &&
								 IsCollectionValid(TokenCollections[collectionName])
					? TokenCollections[collectionName]
					: null;
				if (collection != null)
					collectionName = collection.GetCollectionLabel();
				else if (TokenCollections.ContainsKey(collectionName))
					RemoveCollection(collectionName);
			}
			if (TokenCollections.ContainsKey(collectionName) && TokenCollections[collectionName].IsCurrentContextValid(item) && IsCollectionValid(TokenCollections[collectionName]))
				return TokenCollections[collectionName] as ITokenCollection<T>;
			return null;
		}

		public ITokenCollection<T> GetTokenCollection<T>(ID backingItemId, Item item = null) where T : IToken
		{
			var ret = GetTokenCollections().FirstOrDefault(x => x.GetBackingItemId() == backingItemId) as ITokenCollection<T>;
			if (ret != null)
				return GetTokenCollection<T>(ret.GetCollectionLabel(), item);
			var backingItem = GetDatabase().GetItem(backingItemId);
			if (backingItem == null) return null;
			var collection = GetCollectionFromItem(backingItem);
			if (collection == null) return null;
			if (TokenCollections.ContainsKey(collection.GetCollectionLabel()))
			{
				using (new SecurityDisabler())
				{
					TokenKeeper.CurrentKeeper.GetDatabase().GetItem(backingItemId).Delete();
				}
				throw new TokenException(
					"A token collection was created with a label that already exists in another token collection, please use a different name to create the token collection.");
			}
			LoadTokenCollection(collection);
			return collection as ITokenCollection<T>;
		}

		public virtual IEnumerable<IToken> GetTokens(string category)
		{
			if (!TokenCollections.ContainsKey(category)) return null;
			var collection = TokenCollections[category];
			return collection?.GetTokens();
		}

		public virtual IToken GetToken(string category, string token, Item item = null)
		{
			var collection = GetTokenCollection<IToken>(category, item);
			IToken ret = collection?.GetToken(token);
			return ret;
		}

		public virtual ITokenCollection<IToken> RemoveCollection(string collectionLabel)
		{
			ITokenCollection<IToken> ret;
			TokenCollections.TryRemove(collectionLabel, out ret);
			return ret;
		}

		public virtual void ResetTokenLocations(ID itemId, ID fieldId, Language language, int versionNumber)
		{
			Tuple<DateTime, List<Tuple<int, int>>> ignored;
			string key = itemId.ToString() + fieldId + language.Name + versionNumber;
			if (TokenLocations.ContainsKey(key))
				TokenLocations.TryRemove(key, out ignored);
		}

		public virtual ITokenCollection<IToken> GetCollectionFromItem(Item item)
		{
			var args = new GetTokenCollectionTypeArgs()
			{
				CollectionItem = item
			};
			var pipeline = CorePipelineFactory.GetPipeline("getTokenCollection", string.Empty);
			pipeline.Run(args);
			return args.Collection;
		}

		public virtual TokenDataCollection TokenProperties(string tokenIdentifier)
		{
			if (string.IsNullOrWhiteSpace(tokenIdentifier))
				return new TokenDataCollection();
			var qsRoot = "href=\"/TokenManager";
			int start = tokenIdentifier.IndexOf(qsRoot, StringComparison.Ordinal) + qsRoot.Length;
			int end = -1;
			if (start > qsRoot.Length - 1)
				end = tokenIdentifier.IndexOf('"', start);
			if (start > qsRoot.Length - 1 && end > start)
				return new TokenDataCollection(HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(tokenIdentifier.Substring(start, end - start))));
			return new TokenDataCollection();
		}

		public virtual bool IsInToken(Field field, int startIndex, int length)
		{
			var locations = ParseTokenLocations(field);
			if (locations == null)
				return false;
			foreach (var location in locations.Item2.TakeWhile(location => location.Item1 + location.Item2 >= startIndex))
			{
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
			if (Context.ContentDatabase != null)
				return Context.ContentDatabase;
			if (Context.Database != null)
				return Context.Database;
			var master = Factory.GetDatabase("master", false);
			if (master != null)
				return master;
			if (HttpContext.Current == null && Context.Site != null && Context.Site.Database != null)
				return Context.Site.Database;
			if (HttpContext.Current != null)
			{
				var url = HttpContext.Current.Request.Url;
				var siteContext = Sitecore.Sites.SiteContextFactory.GetSiteContext(url.Host, url.PathAndQuery);
				if (siteContext != null && siteContext.Database != null)
					return siteContext.Database;
			}
			return Factory.GetDatabases().FirstOrDefault(x => x.HasContentItem);
		}

		public virtual bool HasTokens(Item item)
		{
			return
				item.Fields.Where(x => x.Type == "Rich Text")
					.Any(x => TokenLocations.ContainsKey(item.ID.ToString() + x.ID + item.Language.Name + item.Version.Number));
		}

		/// <summary>
		/// finds the token locations and stores it in a cache
		/// </summary>
		/// <param name="item"></param>
		/// <param name="fieldId"></param>
		/// <param name="version"></param>
		/// <param name="text"></param>
		/// <param name="language"></param>
		/// <returns>if there are any tokens found</returns>
		private bool IdentifyTokenLocations(Item item, ID fieldId, Language language, int version, string text)
		{
			var collection = Regex.Matches(text, TokenRegex);
			List<Tuple<int, int>> locations = (from Match m in collection select new Tuple<int, int>(m.Index, FindTokenEnd(m.Index, text))).Reverse().ToList();
			var ret = new Tuple<DateTime, List<Tuple<int, int>>>(item.Statistics.Updated, locations);
			TokenLocations.AddOrUpdate(item.ID + fieldId.ToString() + language.Name + version, ret, (key, value) => ret);
			return locations.Any();
		}

		private int FindTokenEnd(int index, string text)
		{
			int end = text.IndexOf(' ', index);
			string tag = $"</{text.Substring(index + 1, end - index - 1)}>";
			return text.IndexOf(tag, index, StringComparison.CurrentCulture) + tag.Length - index;
		}

		/// <summary>
		/// finds if the locations are cached and if not calls to have them identified
		/// </summary>
		/// <param name="item"></param>
		/// <param name="fieldId"></param>
		/// <param name="version"></param>
		/// <param name="text"></param>
		/// <param name="language"></param>
		/// <returns>if there are any tokens</returns>
		private bool TrackTokens(Item item, ID fieldId, Language language, int version, string text)
		{
			var key = item.ID + fieldId.ToString() + language.Name + version;
			if (TokenLocations.ContainsKey(key) && TokenLocations[key].Item1 == item.Statistics.Updated)
				return TokenLocations[key].Item2.Any();
			return IdentifyTokenLocations(item, fieldId, language, version, text) && TokenLocations[key].Item2.Any();
		}

		/// <summary>
		/// checks if the token collection is valid
		/// </summary>
		/// <param name="collection"></param>
		/// <returns>boolean for if the token is valid</returns>
		private bool IsCollectionValid(ITokenCollection<IToken> collection)
		{
			if (collection.GetBackingItemId() == (ID)null) return true;

			var item = GetDatabase().GetItem(collection.GetBackingItemId());

			if (item != null && item.Statistics.Updated <= TokenCacheUpdateTimes[collection.GetCollectionLabel()]) return true;

			return false;
		}

		/// <summary>
		/// Refreshes the token collection with what's in sitecore
		/// </summary>
		/// <param name="category"></param>
		/// <returns>token collection</returns>
		public ITokenCollection<IToken> RefreshTokenCollection(string category = null)
		{
			RefreshAutoTokens();

			var tokenManagerItems = GetDatabase()
				.SelectItems($"fast:/sitecore/content//*[@@templateid = '{Constants.TokenRootTemplateId}']")
				.Union(Globals.LinkDatabase.GetReferrers(GetDatabase().GetItem(Constants.TokenRootTemplateId))
				.Select(x => GetDatabase().GetItem(x.SourceItemID)));

			HashSet<ID> dups = new HashSet<ID>();
			ITokenCollection<IToken> collection = category != null && TokenCollections.ContainsKey(category) ? TokenCollections[category] : null;

			foreach (Item tokenManagerItem in tokenManagerItems)
			{
				if (tokenManagerItem != null)
				{
					if (dups.Contains(tokenManagerItem.ID)) continue;

					dups.Add(tokenManagerItem.ID);

					Stack<Item> curItems = new Stack<Item>();
					curItems.Push(tokenManagerItem);

					while (curItems.Any())
					{
						Item cur = curItems.Pop();
						// this means that the collection exists, it's just out of date, so we need to update it.
						if (collection != null)
						{
							if (collection.GetBackingItemId() == cur.ID)
							{
								ITokenCollection<IToken> col = GetCollectionFromItem(cur);
								RemoveCollection(category);
								LoadTokenCollection(col);
								return col;
							}
						}
						else
						// this means that the token doesn't exist yet, lets create it.
						{
							ITokenCollection<IToken> col = GetCollectionFromItem(cur);
							if (col != null && col.GetCollectionLabel() == category || category == null)
							{
								LoadTokenCollection(col);
								if (category != null)
									return col;
							}
						}

						foreach (Item child in cur.Children)
						{
							curItems.Push(child);
						}
					}
				}
			}
			return null;
		}

		private void RefreshAutoTokens()
		{
			var dependencyResolver = DependencyResolver.Current;

			var autoTokens = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(x => !Constants.BinaryBlacklist.Contains(x.GetName().Name))
				.SelectMany(GetAutoTokenTypes)
				.Select(t =>
				{
					// try dependency resolver (Note: for 8.2.x with MSDI, you must register the autotoken class with the Sitecore container as if it were say a controller - other IoC containers may not require this)
					if (dependencyResolver != null)
					{
						var result = DependencyResolver.Current.GetService(t) as AutoToken;

						// if the IoC container got something valid we return it
						if (result != null) return result;

						Log.Debug($"AutoToken {t.FullName} was not activated using the MVC DependencyResolver because the dependency resolver returned null for it. It will be activated assuming a parameterless constructor instead.");
					}

					try
					{
						return (AutoToken) Activator.CreateInstance(t);
					}
					catch (MissingMethodException mex)
					{
						throw new InvalidOperationException($"AutoToken {t.FullName} could not be activated. This may indicate a non-parameterless constructor is being used (explicit values need to be set when calling base()). It can also mean that you have IoC dependencies in the constructor, but have not registered the AutoToken class with your IoC container.", mex);
					}
				});

			foreach (AutoToken token in autoTokens)
			{
				TokenKeeper.CurrentKeeper.LoadAutoToken(token);
			}
		}

		private IEnumerable<Type> GetAutoTokenTypes(Assembly a)
		{
			IEnumerable<Type> types = null;
			try
			{
				types = a.GetTypes().Where(t => t.IsSubclassOf(typeof(AutoToken)) && !t.IsAbstract);
			}
			catch (ReflectionTypeLoadException e)
			{
				types = e.Types.Where(t => t != null && t.IsSubclassOf(typeof(AutoToken)) && !t.IsAbstract);
			}

			if (types == null) yield break;

			foreach (var type in types)
			{
				yield return type;
			}
		}
	}
}
