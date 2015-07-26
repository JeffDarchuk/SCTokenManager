using System;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

using Sitecore.Configuration;

using TokenManager.Management;

namespace TokenManager.Handlers
{
	/// <summary>
	/// base handler for http requests to the TokenManager 
	/// </summary>
	public class TokenManagerHandler : BaseHttpHandler
    {
		private readonly string _prefix;

        public TokenManagerHandler() : this(null) { }

        public TokenManagerHandler(string prefix)
        {
			_prefix = prefix;
        }

		/// <summary>
		/// gets post data from post request
		/// </summary>
		/// <param name="context"></param>
		/// <returns>dynamic object containing post javascript object</returns>
		public static dynamic GetPostData(HttpContextBase context)
		{
			using (StreamReader sr = new StreamReader(context.Request.InputStream))
			{
				return JsonConvert.DeserializeObject<ExpandoObject>(sr.ReadToEnd());
			}
		}

		/// <summary>
		/// base HTTP request
		/// </summary>
		/// <param name="context"></param>
        public override void ProcessRequest(HttpContextBase context)
        {
			var path = context.Request.AppRelativeCurrentExecutionFilePath;
			var fileName = Path.GetFileName(path);
			if (fileName == null)
			{
				NotFound(context);
				return;
			}

			var file = fileName.ToLowerInvariant();
			if (_prefix.Equals(file, StringComparison.CurrentCultureIgnoreCase))
			{
				file = "";
			}
			if (string.IsNullOrWhiteSpace(file))
				ReturnResource(context, "index.html", "text/html");
			else if (file.EndsWith(".js"))
				ReturnResource(context, file, "application/javascript");
			else if (file.EndsWith(".html"))
				ReturnResource(context, file, "text/html");
			else if (file.EndsWith(".css"))
				ReturnResource(context, file, "text/css");
			else if (file.EndsWith(".gif"))
				ReturnImage(context, file, ImageFormat.Gif, "image/gif");
			else if (file == "categories.json")
				ReturnJson(context, GetTokenCategories());
			else if (file == "tokens.json")
				ReturnJson(context, GetTokens(context));
			else if (file == "tokenidentifier.json")
				ReturnJson(context, GetTokenIdentifier(context));
			else if (file == "tokenconvert.json")
				ReturnJson(context, ConvertToken(context));
			else if (file == "tokenincorporator.json")
				ReturnJson(context, IncorporateToken(context));
			else if (file == "databases.json")
				ReturnJson(context, GetDatabases());
			else if (file == "sitecoretokencollections.json")
				ReturnJson(context, GetSitecoreTokenCollectionNames(context));
			else if (file == "incorporatetokens.json")
				ReturnJson(context, IncorporateToken(context));
			else if (file == "issitecorecollection.json")
				ReturnJson(context, IsSitecoreCollection(context));
			else if (file == "unziptoken.json")
				ReturnJson(context, UnzipToken(context));
			else if (file == "tokenstats.json")
				ReturnJson(context, GetTokenStats(context));
			else
				NotFound(context);
        }

		/// <summary>
		/// Token Stats Request
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private object GetTokenStats(HttpContextBase context)
		{
			var data = GetPostData(context);
			var tokenStats = new TokenStats(data.category, data.token);
			return tokenStats.GetStats();
		}

		/// <summary>
		/// unzip request
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private object UnzipToken(HttpContextBase context)
		{
			var data = GetPostData(context);
			var unzipper = new TokenUnzipper(data.category, data.token);
			return unzipper.Unzip();
		}

		/// <summary>
		/// is the collection a sitecore collection
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private object IsSitecoreCollection(HttpContextBase context)
		{
			var data = GetPostData(context);
			return
				TokenKeeper.CurrentKeeper.GetTokenCollection(data.category) is SitecoreTokenCollection;
		}

		/// <summary>
		/// get all sitecore managed collectiongs
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private object GetSitecoreTokenCollectionNames(HttpContextBase context)
		{
			var data = GetPostData(context);
			string database = data.database;
			return TokenKeeper.CurrentKeeper.GetTokenCollections().Where(c => c is SitecoreTokenCollection && ((SitecoreTokenCollection) c).IsAvailableOnDatabase(database) ).Select(c=>c.GetCollectionLabel());
		}

		/// <summary>
		/// get all database names
		/// </summary>
		/// <returns></returns>
		private object GetDatabases()
		{
			return Factory.GetDatabases().Select(d => d.Name).ToArray();
		}

		/// <summary>
		/// token incorporator request
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private object IncorporateToken(HttpContextBase context)
		{
			var data = GetPostData(context);
			var incorporator = new TokenIncorporator(data.category, data.tokenName, data.tokenValue);
			return incorporator.Incorporate();
		}

		/// <summary>
		/// convert token request
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private object ConvertToken(HttpContextBase context)
		{
			var data = GetPostData(context);
			var converter = new TokenConverter(data.prefix, data.suffix, data.delimiter);
			return converter.Convert();
		}

		/// <summary>
		/// token idenfier request
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private static object GetTokenIdentifier(HttpContextBase context)
		{
			var data = GetPostData(context);
			return TokenKeeper.CurrentKeeper.GetTokenIdentifier(data.category, data.token);
		}

		/// <summary>
		/// token list request for given category
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private static object GetTokens(HttpContextBase context)
		{
			var data = GetPostData(context);
			return TokenKeeper.CurrentKeeper.GetTokens(data.category);
		}

		/// <summary>
		/// token category name list request
		/// </summary>
		/// <returns></returns>
		private static object GetTokenCategories()
		{
			return TokenKeeper.CurrentKeeper.GetTokenCollectionNames();
		}

    }
}
