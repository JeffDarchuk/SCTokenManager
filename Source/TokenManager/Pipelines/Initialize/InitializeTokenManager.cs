using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Xml;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Engines;
using Sitecore.Data.Fields;
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
		private static bool _needRebuild;
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
				TokenKeeper.TokenSingleton = Tokens;
			}

			Assert.ArgumentNotNull(args, "args");

			RegisterRoutes("tokenManager");

			TokenKeeper._isSc8 = IsSc8();

			if (RequiredSitecoreItemsMissing())
			{
				var filepath = "";
				if (System.Text.RegularExpressions.Regex.IsMatch(Settings.DataFolder, @"^(([a-zA-Z]:\\)|(//)).*"))
					//if we have an absolute path, rather than relative to the site root
					filepath = Settings.DataFolder +
							   @"\packages\TokenManager.TokenManagerPackage.zip";
				else
					filepath = HttpRuntime.AppDomainAppPath + Settings.DataFolder.Substring(1) +
							   @"\packages\TokenManager.TokenManagerPackage.zip";
				try
				{
					using (var manifestResourceStream = GetType().Assembly
						.GetManifestResourceStream(TokenKeeper.IsSc8
							? "TokenManager.Resources.TokenManagerPackage.zip"
							: "TokenManager.Resources.TokenManagerSc7.zip"))
					using (var file = new FileStream(filepath, FileMode.Create))
					{
						manifestResourceStream?.CopyTo(file);
					}

					int count = 0;
					while (true)
					{
						if (!IsFileLocked(new FileInfo(filepath)))
						{
							using (new SecurityDisabler())
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


						Thread.Sleep(1000);
						count++;
						if (count > 15)
							break;
					}

					RegisterSitecoreTokens();
					ValidateInsertOptions();
				}
				catch (Exception e)
				{
					Log.Error("TokenManager was unable to initialize", e, this);
				}
			}
			else
			{
				RegisterSitecoreTokens();
				ValidateInsertOptions();
			}
		}

		private static bool IsSc8()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(HttpRuntime.AppDomainAppPath + "/sitecore/shell/sitecore.version.xml");
			var selectSingleNode = doc.SelectSingleNode("/information/version/major");
			int version;
			int.TryParse(selectSingleNode.InnerText, out version);
			return selectSingleNode != null && version > 7;
		}

		private static void ValidateInsertOptions()
		{
			Item sv = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(Constants.TokenCollectionStandardValuesId);
			if (sv == null)
			{
				_needRebuild = true;
			}
			else
			{
				MultilistField fld = sv.Fields[FieldIDs.Branches];
				HashSet<ID> existing = new HashSet<ID>(fld.TargetIDs);
				var missingTokens = TokenIdentifier.Current.GetAllTokenTemplates().Where(x => !existing.Contains(x)).ToList();
				if (missingTokens.Any())
				{
					StringBuilder sb = new StringBuilder();
					if (sb.Length == 0)
					{
						sb.Append(string.Join("|", fld.TargetIDs.Select(x => x.ToString())));
					}

					foreach (ID needsToBeAdded in missingTokens)
					{
						sb.Append("|").Append(needsToBeAdded);
					}

					using (new SecurityDisabler())
					{
						using (new EditContext(sv))
						{
							sv[FieldIDs.Branches] = sb.ToString();
						}
					}
				}
			}
		}
		/// <summary>
		/// Detects if a required sitecore item is missing.
		/// </summary>
		/// <returns></returns>
		private static bool RequiredSitecoreItemsMissing()
		{
			if (_needRebuild)
			{
				return true;
			}

			return typeof(Constants)
				.GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.FieldType == typeof(string))
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
			Tokens.RefreshTokenCollection();
		}

		/// <summary>
		/// Registers the MVC routes for the TokenManager web app
		/// </summary>
		/// <param name="route"></param>
		public static void RegisterRoutes(string route)
		{
			if (Factory.GetDatabase("master", false) == null) return;

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
