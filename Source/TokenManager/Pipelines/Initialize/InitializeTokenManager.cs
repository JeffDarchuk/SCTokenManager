using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

using TokenManager.Data.Interfaces;
using TokenManager.Handlers;
using TokenManager.Management;

namespace TokenManager.Pipelines.Initialize
{

	public class InitializeTokenManager
	{
		private const string _tokenManagerGuid = "{AC075339-E1DD-424C-9EFB-FD47CCEB6FD9}";
		private TokenKeeper Tokens { get; set; }
		/// <summary>
		/// Starts up a new tokenkeeper that will be set up as a singleton
		/// </summary>
		/// <param name="args"></param>
		public void Process(PipelineArgs args)
		{
			if (TokenKeeper.TokenSingleton == null)
			{
				RegisterSitecoreTokens();
				TokenKeeper.TokenSingleton = Tokens;
			}

			Assert.ArgumentNotNull(args, "args");
			RegisterRoutes("tokenManager");
		}

		/// <summary>
		/// from the token manager it finds and incorporates all initial token categories
		/// </summary>
		private void RegisterSitecoreTokens()
		{
			Item tokenManagerItem = Database.GetDatabase("master").GetItem(_tokenManagerGuid);
			if (tokenManagerItem != null)
			{

				Stack<Item> curItems = new Stack<Item>();
				curItems.Push(tokenManagerItem);
				while (curItems.Any())
				{
					Item cur = curItems.Pop();
					ITokenCollection col = Tokens.GetCollectionFromItem(cur);
					if (col != null)
						Tokens.LoadTokenGroup(col);
					else
						foreach (Item child in cur.Children)
							curItems.Push(child);
				}
			}
		}

		/// <summary>
		/// Registers the MVC routes for the TokenManager web app
		/// </summary>
		/// <param name="route"></param>
		public static void RegisterRoutes(string route)
		{
			var routes = RouteTable.Routes;
			var handler = new TokenManagerHandler(route);
			using (routes.GetWriteLock())
			{
				var defaultRoute = new Route(route + "/", handler)
				{
					// we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
					Defaults = new RouteValueDictionary(new { controller = "TokenManagerHandler", action = "ProcessRequest" }),
					Constraints = new RouteValueDictionary(new { controller = "TokenManagerHandler", action = "ProcessRequest" })
				};

				routes.Add("TokenManagerHandlerRoute", defaultRoute);

				var filenameRoute = new Route(route + "/{filename}", handler)
				{
					Defaults = new RouteValueDictionary(new { controller = "TokenManagerHandler", action = "ProcessRequest" }),
					Constraints = new RouteValueDictionary(new { controller = "TokenManagerHandler", action = "ProcessRequest" })
				};

				routes.Add("TokenManagerHandlerFilenameRoute", filenameRoute);
			}
		}
	}
}
