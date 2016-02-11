using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Managers;
using Sitecore.Mvc.Extensions;
using Sitecore.SecurityModel;
using TokenManager.Management;

namespace TokenManager.Handlers.TokenOperations
{
	public class TokenRootPropertyChanger
	{
		private readonly string _category;
		private readonly string _token;

		public TokenRootPropertyChanger(string category, string token)
		{
			_category = category;
			_token = token;
		}

		public dynamic Change(string newCategory, string newToken)
		{
			dynamic ret = new ExpandoObject();
			ret.Changed = 0;
			ret.Converted = new List<ExpandoObject>();
			using (new SecurityDisabler())
			{
				foreach (var current in TokenKeeper.CurrentKeeper.GetTokenOccurances(_category, _token))
				{
					var item = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(ID.Parse(current.Id), !string.IsNullOrWhiteSpace(current.Language) && LanguageManager.IsValidLanguageName(current.Language) ? LanguageManager.GetLanguage(current.Language) : LanguageManager.DefaultLanguage);

					if (item != null)
					{
						item.Editing.BeginEdit();
						foreach (Field field in item.Fields.Where(f => f.Type == "Rich Text"))
						{
							ret.Changed++;
							dynamic fieldReport = new ExpandoObject();
							fieldReport.DisplayName = field.Item.DisplayName;
							fieldReport.ID = field.Item.ID;
							fieldReport.Path = field.Item.Paths.FullPath;
							fieldReport.FieldName = field.Name;
							fieldReport.InstancesReplaced = 0;
							fieldReport.Language = field.Language.Name;
							foreach (var tokenLocation in TokenKeeper.CurrentKeeper.ParseTokenLocations(field).Item2)
							{
								var tokenIdentifier = field.Value.Substring(tokenLocation.Item1, tokenLocation.Item2);
								var tokenProps = TokenKeeper.CurrentKeeper.TokenProperties(tokenIdentifier);
								if (tokenProps["Category"] != _category || tokenProps["Token"] != _token)
									continue;
								fieldReport.InstancesReplaced++;

								StringBuilder sb = new StringBuilder(field.Value);
								sb.Remove(tokenLocation.Item1, tokenLocation.Item2);
								sb.Insert(tokenLocation.Item1, TokenKeeper.CurrentKeeper.GetTokenIdentifier(newCategory, newToken, tokenProps.AllKeys.ToDictionary(k => k, k => (object)tokenProps[k])));
								field.Value = sb.ToString();

							}
							ret.Converted.Add(fieldReport);
						}
						item.Editing.EndEdit();
					}
				}
			}
			return ret;
		}
	}
}
