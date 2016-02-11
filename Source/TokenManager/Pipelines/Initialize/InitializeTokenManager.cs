using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Engines;
using Sitecore.Data.Items;
using Sitecore.Data.Proxies;
using Sitecore.Diagnostics;
using Sitecore.Install.Files;
using Sitecore.Install.Framework;
using Sitecore.Install.Items;
using Sitecore.Install.Utils;
using Sitecore.Pipelines;
using Sitecore.SecurityModel;
using TokenManager.Data.Interfaces;
using TokenManager.Handlers;
using TokenManager.Management;

namespace TokenManager.Pipelines.Initialize
{

	public class InitializeTokenManager
	{
		private ITokenKeeperService Tokens { get; set; }
		private ITokenIdentifier Identifier { get; set; }

		/// <summary>
		/// Starts up a new tokenkeeper that will be set up as a singleton
		/// </summary>
		/// <param name="args"></param>
		public void Process(PipelineArgs args)
		{
			if (TokenIdentifier.Current == null)
			{
				TokenIdentifier.IdentifierSingleton = Identifier;
			}
			if (TokenKeeper.TokenSingleton == null)
			{
				RegisterSitecoreTokens();
				TokenKeeper.TokenSingleton = Tokens;
			}

			Assert.ArgumentNotNull(args, "args");
			RegisterRoutes("tokenManager");

			if (RequiredSitecoreItemsMissing())
			{
				var filepath = "";
				if (System.Text.RegularExpressions.Regex.IsMatch(Settings.DataFolder, @"^(([a-zA-Z]:\\)|(//)).*")) //if we have an absolute path, rather than relative to the site root
					filepath = Settings.DataFolder +
							   @"\packages\TokenManager.TokenManagerPackage.zip";
				else
					filepath = HttpRuntime.AppDomainAppPath + Settings.DataFolder.Substring(1) +
							   @"\packages\TokenManager.TokenManagerPackage.zip";
				try
				{
					var manifestResourceStream = GetType().Assembly
						.GetManifestResourceStream("TokenManager.Resources.TokenManagerPackage.zip");
					manifestResourceStream?.CopyTo(new FileStream(filepath, FileMode.Create));
					Task.Run(() =>
					{

						while (true)
						{
							if (!IsFileLocked(new FileInfo(filepath)))
							{

								using (new SecurityDisabler())
								{
									using (new ProxyDisabler())
									{
										using (new SyncOperationContext())
										{
											IProcessingContext context = new SimpleProcessingContext();
											IItemInstallerEvents events =
												new DefaultItemInstallerEvents(
													new BehaviourOptions(InstallMode.Overwrite, MergeMode.Undefined));
											context.AddAspect(events);
											IFileInstallerEvents events1 = new DefaultFileInstallerEvents(true);
											context.AddAspect(events1);

											Sitecore.Install.Installer installer = new Sitecore.Install.Installer();
											installer.InstallPackage(MainUtil.MapPath(filepath), context);
											break;
										}
									}
								}
							}
							else
								Thread.Sleep(1000);
						}
					});
				}
				catch (Exception e)
				{
					Log.Error("TokenManager was unable to initialize", e, this);
				}
			}
		}

		/// <summary>
		/// Detects if a required sitecore item is missing.
		/// </summary>
		/// <returns></returns>
		private static bool RequiredSitecoreItemsMissing()
		{
			return typeof(Constants)
				.GetFields(BindingFlags.Static | BindingFlags.Public)
				.Any(f => TokenKeeper.CurrentKeeper.GetDatabase().GetItem(f.GetValue(null).ToString()) == null);
		}

		/// <summary>
		/// checks to see if the file is done being written to the filesystem
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		protected virtual bool IsFileLocked(FileInfo file)
		{
			FileStream stream = null;

			try
			{
				stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
			}
			catch (IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}
			finally
			{
				stream?.Close();
			}

			//file is not locked
			return false;
		}

		/// <summary>
		/// from the token manager it finds and incorporates all initial token categories
		/// </summary>
		private void RegisterSitecoreTokens()
		{
			Item[] tokenManagerItems =
				Tokens.GetDatabase().SelectItems($"fast:/sitecore/content//*[@@templateid = '{Constants.TokenRootTemplateId}']");
			foreach (Item tokenManagerItem in tokenManagerItems)
			{
				if (tokenManagerItem == null) return;
				Stack<Item> curItems = new Stack<Item>();
				curItems.Push(tokenManagerItem);
				while (curItems.Any())
				{
					Item cur = curItems.Pop();
					ITokenCollection<IToken> col = Tokens.GetCollectionFromItem(cur);
					if (col != null)
						Tokens.LoadTokenCollection(col);
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
