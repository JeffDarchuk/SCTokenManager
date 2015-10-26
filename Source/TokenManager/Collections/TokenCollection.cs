using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Collections
{

	public abstract class TokenCollection<T> : ITokenCollection<T>
		where T : IToken
	{
		private string _ancestorPath;
		private string _templateId;
		private readonly Dictionary<string, T> _tokens = new Dictionary<string, T>();
		private readonly Dictionary<ID, DateTime> CacheTime = new Dictionary<ID, DateTime>();
		private ID _itemID;
		private DateTime _itemUpdated;

		/// <summary>
		/// used for lazy loading the token values
		/// </summary>
		/// <param name="token"></param>
		/// <returns>token object</returns>
		public abstract T InitiateToken(string token);

		public T this[string token] => GetToken(token);

		public string SitecoreIcon { get; set; }

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
			InitializeCollection(backingItem);
		}

		private void InitializeCollection(Item backingItem)
		{
			_itemID = backingItem.ID;
			var tmp = backingItem.Database.GetItem(backingItem["Item Ancestor"]);
			if (tmp != null)
				_ancestorPath = tmp.Paths.Path;
			_templateId = backingItem["Item Template"];
			_itemUpdated = backingItem.Statistics.Updated;
		}

		/// <summary>
		/// constructs the token collection based on the backing item and a collection of initial tokens
		/// </summary>
		/// <param name="backingItem"></param>
		/// <param name="tokensToLoad"></param>
		public TokenCollection(Item backingItem, IEnumerable<T> tokensToLoad)
			: this(backingItem)
		{
			InitializeCollection(backingItem);
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
			return GetToken(token) != null;
		}

		/// <summary>
		/// returns the token based on the text
		/// </summary>
		/// <param name="token"></param>
		/// <returns>token object</returns>
		public virtual T GetToken(string token)
		{
			var item = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(_itemID);
			if (item == null)
				ResetTokenCache();
			else if (item.Statistics.Updated > _itemUpdated)
			{
				InitializeCollection(item);
				ResetTokenCache();
			}
			if (_tokens.ContainsKey(token))
			{
				IToken t = _tokens[token];
				if (t != null)
				{
					if (t.GetBackingItemId() != (ID)null)
					{
						var tokenItem = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(t.GetBackingItemId());
						if (tokenItem != null)
							if (!CacheTime.ContainsKey(t.GetBackingItemId()) || tokenItem.Statistics.Updated > CacheTime[t.GetBackingItemId()])
							{
								_tokens[token] = InitiateToken(token);
								CacheTime[t.GetBackingItemId()] = tokenItem.Statistics.Updated;
								return _tokens[token];
							}
					}
					else
					{
						return _tokens[token];
					}

				}
			}
			_tokens[token] = InitiateToken(token);
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
		public virtual string GetTokenValue(string token, NameValueCollection extraData)
		{
			var ret = GetToken(token);
			return ret == null ? string.Empty : ret.Value(extraData);
		}

		/// <summary>
		/// get all token names
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IToken> GetTokens()
		{
			return _tokens.Where(x => x.Value != null).Select(x => x.Value as IToken);
		}

		/// <summary>
		/// find out if the context item is a valid context for this collection
		/// </summary>
		/// <param name="item"></param>
		/// <returns>is the collection valid in the current location</returns>
		public virtual bool IsCurrentContextValid(Item item = null)
		{
			if (item == null)
			{
				item = Context.Item;
			}
			if (item == null || item.Database.Name != TokenKeeper.CurrentKeeper.GetDatabase().Name)
				return true;
			var collectionItem = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(_itemID);
			if (collectionItem != null &&
				collectionItem.Parent.TemplateID.ToString() == Constants.TokenCollectionFolderTemplateId)
			{
				var parent = collectionItem.Parent;
				var tmp = parent.Database.GetItem(parent["Item Ancestor"]);
				var parentFilterPath = "";
				if (tmp != null)
					parentFilterPath = tmp.Paths.Path;
				var parentFilterTemplateId = parent["Item Template"];
				if (!string.IsNullOrWhiteSpace(parentFilterPath) && !item.Paths.Path.StartsWith(parentFilterPath))
					return false;
				if (!string.IsNullOrWhiteSpace(parentFilterTemplateId) && item.TemplateID.ToString() != parentFilterTemplateId)
					return false;
			}
			if (string.IsNullOrWhiteSpace(_ancestorPath) && string.IsNullOrWhiteSpace(_templateId))
				return true;
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
		/// get the item id for the sitecore item corresponding to this collection
		/// </summary>
		/// <returns>backing item id</returns>
		public virtual ID GetBackingItemId()
		{
			return _itemID;
		}
	}
}
