using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore;
using Sitecore.Data;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data.Tokens
{

    public class SitecoreToken : IToken
    {
        private Dictionary<string, string> _databaseToValue = new Dictionary<string, string>();
        private object locker = new object();
        private ID _backingItem;
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
            Database db = Context.ContentDatabase ?? Context.Database ?? Database.GetDatabase("master");
            var lang = string.IsNullOrWhiteSpace(extraData["Language"]) ? Context.Language.Name : extraData["Language"];
            if (_databaseToValue.ContainsKey(db.Name + lang))
                return _databaseToValue[db.Name + lang];
            if (LoadValue(db, lang))
                return _databaseToValue[db.Name + lang];
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
        /// <returns>was the token found</returns>
        private bool LoadValue(Database db, string language)
        {
            lock (locker)
            {
                if (db != null)
                {
                    if (!_databaseToValue.ContainsKey(db.Name))
                    {

                        var item = db.GetItem(_backingItem);
                        _databaseToValue[db.Name + language] = item["Value"];
                        return true;
                    }
                    _databaseToValue[db.Name + language] = null;
                }
                
            }
            return false;
        }
    }
}
