using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore;
using Sitecore.Data;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{

	public class SitecoreToken : SitecoreBasedToken
	{
		private readonly ConcurrentDictionary<string, string> _databaseToValue = new ConcurrentDictionary<string, string>();

		public SitecoreToken(string name, ID backingId) : base(name, backingId)
		{
		}

		public override IEnumerable<ITokenData> ExtraData()
		{
			return null;
		}


		public override string Value(NameValueCollection extraData)
		{
			Database db = TokenKeeper.CurrentKeeper.GetDatabase();
			var lang = Context.Language.Name;
			int? version = db?.GetItem(GetBackingItemId())?.Version.Number;
			var key = db?.Name + lang + version;
			if (_databaseToValue.ContainsKey(key))
				return _databaseToValue[key];
			if (LoadValue(db, key))
				return _databaseToValue[key];
			return null;
		}

		/// <summary>
		/// caches the token value for the spedified database
		/// </summary>
		/// <param name="db"></param>
		/// <param name="key"></param>
		/// <returns>was the token found</returns>
		private bool LoadValue(Database db, string key)
		{
			if (db != null)
			{
				if (!_databaseToValue.ContainsKey(db.Name))
				{

					var item = db.GetItem(BackingId);
					var val = item["Value"];
					if (item["Strip Outer P Tags"] == "1" && val.StartsWith("<p>") && val.EndsWith("</p>"))
						val = val.Substring(3, val.Length - 7);
					_databaseToValue[key] = val;
					return true;
				}
				_databaseToValue[key] = null;
			}
			return false;
		}
	}
}
