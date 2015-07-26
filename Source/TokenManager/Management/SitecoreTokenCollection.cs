using System;
using System.Collections.Generic;
using System.Linq;

using Sitecore.Data;
using Sitecore.Data.Items;

using TokenManager.Data;

namespace TokenManager.Management
{

	public class SitecoreTokenCollection : TokenCollection<SitecoreToken>
	{
		private readonly ID _backingItemId;
		private readonly string _collectionLabel;
		private readonly string _ancestorPath;
		private readonly string _templateId;
		private bool _initialized;
		private readonly object _locker = new object();

		/// <summary>
		/// constructs the sitecore token collection based on the specific item
		/// </summary>
		/// <param name="tokenGroup"></param>
		public SitecoreTokenCollection(Item tokenGroup):base(tokenGroup)
		{
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
		/// loads in the token to the collection
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public override SitecoreToken InitiateToken(string token)
		{
			Database db = GetDatabase();
			Item tokenItem = db.GetItem(_backingItemId).Children.FirstOrDefault(i => i["Token"] == token);
			if (tokenItem == null)
				return null;
			return new SitecoreToken(token, tokenItem.ID);
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
						foreach (string key in item.Children.Where(c => c.TemplateID.ToString() == "{87BFAA2C-2E2F-42C6-A135-9F2AE7D32807}" || c.TemplateID.ToString() == "{D2C980F1-DD2A-4444-AC67-EC6D282B5879}").Select(c => c["Token"]))
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
