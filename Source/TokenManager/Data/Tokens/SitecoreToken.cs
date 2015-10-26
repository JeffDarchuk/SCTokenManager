using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore;
using Sitecore.Data;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Data.Tokens
{

    public class SitecoreToken : IToken
    {
        private readonly ConcurrentDictionary<string, string> _databaseToValue = new ConcurrentDictionary<string, string>();
	    private readonly ID _backingItem;
        public string Token { get; set; }

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

        public IEnumerable<ITokenData> ExtraData()
        {
            return null;
        }

        /// <summary>
        /// returns id for sitecore backing item
        /// </summary>
        /// <returns>sitecore item id</returns>
        public ID GetBackingItemId()
        {
            return _backingItem;
        }

        public string Value(NameValueCollection extraData)
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

        public IToken LoadExtraData(NameValueCollection props)
        {
            return this;
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

                    var item = db.GetItem(_backingItem);
                    _databaseToValue[key] = item["Value"];
                    return true;
                }
                _databaseToValue[key] = null;
            }
            return false;
        }
    }
}
