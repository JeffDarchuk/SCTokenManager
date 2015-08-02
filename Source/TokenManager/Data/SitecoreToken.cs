using System.Collections.Generic;

using Sitecore;
using Sitecore.Data;

using TokenManager.Data.Interfaces;

namespace TokenManager.Data
{

	public class SitecoreToken : IToken
	{
		private Dictionary<string, string> _databaseToValue = new Dictionary<string, string>();
		private object locker = new object();
		private ID _backingItem;
		public string Token { get; set; }
		public string Value
		{
			get
			{
				Database db = Context.ContentDatabase ??  Context.Database ?? Database.GetDatabase("master");
				if (_databaseToValue.ContainsKey(db.Name))
					return _databaseToValue[db.Name];
				if (LoadValue(db))
					return _databaseToValue[db.Name];
				return null;
			}
		}

		/// <summary>
		/// sets up the token with where to look in sitecore for the token value, to be lazy loaded later
		/// </summary>
		/// <param name="token"></param>
		/// <param name="scId"></param>
		public SitecoreToken(string token, ID scId)
		{
			Token = token;
			_backingItem = scId;
		}

		/// <summary>
		/// returns id for sitecore backing item
		/// </summary>
		/// <returns>sitecore item id</returns>
		public ID GetBackingItemId()
		{
			return _backingItem;
		}

		/// <summary>
		/// caches the token value for the spedified database
		/// </summary>
		/// <param name="db"></param>
		/// <returns>was the token found</returns>
		private bool LoadValue(Database db)
		{
			lock (locker)
			{
				if (!_databaseToValue.ContainsKey(db.Name))
				{
					if (db != null)
					{
						var item = db.GetItem(_backingItem);
						_databaseToValue[db.Name] = item["Value"];
						return true;
					}
				}
				_databaseToValue[db.Name] = null;
			}
			return false;
		}
	}
}
