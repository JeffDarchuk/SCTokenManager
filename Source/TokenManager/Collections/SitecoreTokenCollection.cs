using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Collections
{

	public abstract class SitecoreTokenCollection<T> : TokenCollection<T>
		where T : IToken
	{
		private readonly ID _backingItemId;
		private readonly string _collectionLabel;

		/// <summary>
		/// constructs the sitecore token collection based on the specific item
		/// </summary>
		/// <param name="tokenCollection"></param>
		protected SitecoreTokenCollection(Item tokenCollection) : base(tokenCollection)
		{
			_backingItemId = tokenCollection.ID;
			_collectionLabel = tokenCollection["Category Label"];

			if (string.IsNullOrWhiteSpace(_collectionLabel))
			{
				throw new ArgumentException("Category labels can not be empty", _backingItemId.ToString());
			}

			SitecoreIcon = tokenCollection[FieldIDs.Icon];

			if (string.IsNullOrWhiteSpace(SitecoreIcon))
			{
				SitecoreIcon = tokenCollection.Template.Icon;
			}
		}

		/// <summary>
		/// get collection label
		/// </summary>
		/// <returns>collection label</returns>
		public override string GetCollectionLabel()
		{
			return _collectionLabel;
		}

		/// <summary>
		/// clears all tokens cached because something has changed
		/// </summary>
		public override void ResetTokenCache()
		{
			RemoveAllTokens();
		}

		/// <summary>
		/// loads in all tokens, stores them in the cache, and returns them
		/// </summary>
		/// <returns>all tokens</returns>
		public override IEnumerable<IToken> GetTokens()
		{
			Database db = TokenKeeper.CurrentKeeper.GetDatabase();
			var item = db.GetItem(_backingItemId);

			if (item != null)
			{
				foreach (string key in item.Children.Select(c => c["Token"]))
				{
					AddOrUpdateToken(InitiateToken(key));
				}
			}

			return base.GetTokens();
		}

		/// <summary>
		/// identifies if the token category is available on the specified database
		/// </summary>
		/// <param name="database"></param>
		/// <returns>is the backing item available on the database</returns>
		public bool IsAvailableOnDatabase(string database)
		{
			Database db = TokenKeeper.CurrentKeeper.GetDatabase(); ;
			return db != null && db.GetItem(_backingItemId) != null;
		}


	}
}
