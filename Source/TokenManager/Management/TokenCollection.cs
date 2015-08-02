using System.Collections.Generic;
using System.Linq;
using System.Web;

using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;

using TokenManager.Data.Interfaces;

namespace TokenManager.Management
{

	public abstract class TokenCollection<T> : ITokenCollection<T>
		where T : IToken
	{
		private readonly string _ancestorPath;
		private readonly string _templateId;
		private readonly Dictionary<string, T> _tokens = new Dictionary<string, T>();
		private readonly ID _itemID;

		/// <summary>
		/// used for lazy loading the token values
		/// </summary>
		/// <param name="token"></param>
		/// <returns>token object</returns>
		public abstract T InitiateToken(string token);

		public string this[string token]
		{
			get { return GetTokenValue(token); }
		}

		/// <summary>
		/// finds the collection label
		/// </summary>
		/// <returns>the collection label</returns>
		public abstract string GetCollectionLabel();

		/// <summary>
		/// instructs to clear any internal caches holding token information
		/// </summary>
		public abstract void ResetTokenCache();

		/// <summary>
		/// constructs the token collection based on the backing item
		/// </summary>
		/// <param name="backingItem"></param>
		public TokenCollection(Item backingItem)
		{
			_itemID = backingItem.ID;
			var tmp = backingItem.Database.GetItem(backingItem["Item Ancestor"]);
			if (tmp != null)
				_ancestorPath = tmp.Paths.Path;
			_templateId = backingItem["Item Template"];
		}

		/// <summary>
		/// constructs the token collection based on the backing item and a collection of initial tokens
		/// </summary>
		/// <param name="backingItem"></param>
		/// <param name="tokensToLoad"></param>
		public TokenCollection(Item backingItem, IEnumerable<T> tokensToLoad):this(backingItem)
		{
			foreach (var token in tokensToLoad)
				_tokens.Add(token.Token, token);
		}

		/// <summary>
		/// collection has given token
		/// </summary>
		/// <param name="token"></param>
		/// <returns>collection has token</returns>
		public virtual bool HasToken(string token)
		{
			return GetToken(token) !=null;
		}

		/// <summary>
		/// returns the token based on the text
		/// </summary>
		/// <param name="token"></param>
		/// <returns>token object</returns>
		public virtual T GetToken(string token)
		{
			if (_tokens.ContainsKey(token)) return _tokens[token];
			var newToken = InitiateToken(token);
			_tokens[token] = newToken;
			return _tokens[token];
		}

		/// <summary>
		/// adds a new token or updates an existing token
		/// </summary>
		/// <param name="oldToken"></param>
		/// <param name="newToken"></param>
		public virtual void AddOrUpdateToken(string oldToken, T newToken)
		{
			if (oldToken != null && oldToken != newToken.Token)
				RemoveToken(oldToken);
			_tokens[newToken.Token] = newToken;
		}

		/// <summary>
		/// removes the token from the collection
		/// </summary>
		/// <param name="token"></param>
		public virtual void RemoveToken(string token)
		{
			if (_tokens.ContainsKey(token))
				_tokens.Remove(token);
		}

		/// <summary>
		/// gets token value
		/// </summary>
		/// <param name="token"></param>
		/// <returns>value of given token</returns>
		public virtual string GetTokenValue(string token)
		{
			var ret = GetToken(token);
			return ret == null ? string.Empty : ret.Value;
		}

		/// <summary>
		/// get all token names
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<string> GetTokens()
		{
			return _tokens.Where(x=>x.Value != null).Select(x=>x.Key);
		}

		/// <summary>
		/// find out if the context item is a valid context for this collection
		/// </summary>
		/// <param name="item"></param>
		/// <returns>is the collection valid in the current location</returns>
		public virtual bool IsCurrentContextValid(Item item = null)
		{
			if (string.IsNullOrWhiteSpace(_ancestorPath) && string.IsNullOrWhiteSpace(_templateId))
				return true;
			if (item == null)
			{
				item = Context.Item;
				if (item == null)
				{
					var itemId = HttpContext.Current.Request.QueryString.Get("sc_itemid");
					if (string.IsNullOrWhiteSpace(itemId))
						return true;
					item = GetDatabase().GetItem(itemId);
					if (item == null)
						return true;
				}
			}
			if ((string.IsNullOrWhiteSpace(_ancestorPath) || item.Paths.Path.StartsWith(_ancestorPath)) &&
				(string.IsNullOrWhiteSpace(_templateId) || item.TemplateID.ToString() == _templateId))
				return true;

			return false;
		}

		/// <summary>
		/// adds or updates the token assuming that the token name didn't change
		/// </summary>
		/// <param name="newToken"></param>
		public virtual void AddOrUpdateToken(T newToken)
		{
			AddOrUpdateToken(null, newToken);
		}

		/// <summary>
		/// clears all tokens
		/// </summary>
		public virtual void RemoveAllTokens()
		{
			_tokens.Clear();
		}

		/// <summary>
		/// gets the most appropriate database
		/// </summary>
		/// <returns></returns>
		public virtual Database GetDatabase()
		{
			return Context.ContentDatabase ?? Context.Database ?? Database.GetDatabase("master");
		}

		/// <summary>
		/// get the item id for the sitecore item corresponding to this collection
		/// </summary>
		/// <returns>backing item id</returns>
		public ID GetBackingItemId()
		{
			return _itemID;
		}
	}
}
