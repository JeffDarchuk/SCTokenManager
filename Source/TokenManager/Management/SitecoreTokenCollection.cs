using System;
using System.Collections.Generic;
using System.Linq;

using Sitecore.Data;
using Sitecore.Data.Items;

using TokenManager.Data;
using TokenManager.Data.Interfaces;

namespace TokenManager.Management
{

	public abstract class SitecoreTokenCollection<T> : TokenCollection<T>
		where T : IToken
	{
		private readonly ID _backingItemId;
		private readonly string _collectionLabel;
		private readonly string _ancestorPath;
		private readonly string _templateId;
		private bool _initialized;
		private readonly object _locker = new object();
	    private ID _tokenTemplateID;

		/// <summary>
		/// constructs the sitecore token collection based on the specific item
		/// </summary>
		/// <param name="tokenGroup"></param>
		public SitecoreTokenCollection(Item tokenGroup, ID tokenTemplateID):base(tokenGroup)
		{
		    _tokenTemplateID = tokenTemplateID;
			_backingItemId = tokenGroup.ID;
			_collectionLabel = tokenGroup["Category Label"];
			var tmp = tokenGroup.Database.GetItem(tokenGroup["Item Ancestor"]);
			if (tmp != null)
				_ancestorPath = tmp.Paths.Path;
			_templateId = tokenGroup["Item Template"];
			if (string.IsNullOrWhiteSpace(_collectionLabel))
				throw new ArgumentException("Category labels can not be empty", _backingItemId.ToString());
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
			_initialized = false;
			RemoveAllTokens();
		}

		/// <summary>
		/// loads in all tokens, stores them in the cache, and returns them
		/// </summary>
		/// <returns>all tokens</returns>
		public override IEnumerable<string> GetTokens()
		{
			if (!_initialized)
			{
				lock (_locker)
				{
					if (!_initialized)
					{
						_initialized = true;
						Database db = GetDatabase();
						var item = db.GetItem(_backingItemId);
						foreach (string key in item.Children.Where(c => c.TemplateID == _tokenTemplateID).Select(c => c["Token"]))
						{
							AddOrUpdateToken(InitiateToken(key));
						}
					}
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
			Database db = GetDatabase();
			return db != null && db.GetItem(_backingItemId) != null;
		}


	}
}
