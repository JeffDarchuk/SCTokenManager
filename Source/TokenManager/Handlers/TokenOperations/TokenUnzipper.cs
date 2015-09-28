using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.SecurityModel;
using TokenManager.ContentSearch;
using TokenManager.Management;

namespace TokenManager.Handlers.TokenOperations
{
	/// <summary>
	/// unzipping here referrs to the process of replacing all token identifiers with their value
	/// thus effictively "untokening" the token
	/// </summary>
	class TokenUnzipper
	{
		private readonly string _category;
		private readonly string _token;
	    private readonly string _root;
	    private readonly bool _replaceWithValue;
		private readonly Database _database;

	    /// <summary>
	    /// constructs an unzipper for the specified token identifying information
	    /// </summary>
	    /// <param name="root">root item ID for operation</param>
	    /// <param name="category">category of token to unzip</param>
	    /// <param name="token">token name to unzip</param>
	    /// <param name="replaceWithValue">replace the token identifier with it's value</param>
	    public TokenUnzipper(string root, string category, string token, bool replaceWithValue)
		{
		    _root = root;
			_category = category;
			_token = token;
			_database = Database.GetDatabase("master");
		    _replaceWithValue = replaceWithValue;
		}

		/// <summary>
		/// removes all instances of this token, replacing them with the token value
		/// </summary>
		/// <returns>dynamic object with statistics of what transpired</returns>
		public dynamic Unzip(bool preview = false)
		{
			dynamic ret = new ExpandoObject();
			ret.Count = 0;
			ret.Converted = new List<ExpandoObject>();
			using (new SecurityDisabler())
			{
                foreach (ContentSearchTokens current in TokenKeeper.CurrentKeeper.GetTokenOccurances(_category, _token, new ID(_root)))
				{
					Item item = _database.GetItem(ID.Parse(current.Id), !string.IsNullOrWhiteSpace(current.Language) && LanguageManager.IsValidLanguageName(current.Language) ? LanguageManager.GetLanguage(current.Language): LanguageManager.DefaultLanguage );
					foreach (var field in item.Fields.Where(f => f.Type == "Rich Text"))
						ProcessRte(field, ret, preview);
				}
			}
			return ret;
		}

	    /// <summary>
	    /// process this rich text field looking for tokens to wipe out
	    /// </summary>
	    /// <param name="field"></param>
	    /// <param name="ret"></param>
	    /// <param name="preview"></param>
	    private void ProcessRte(Field field, dynamic ret, bool preview)
		{
		    ret.Count++;
            dynamic fieldReport = new ExpandoObject();
            fieldReport.DisplayName = field.Item.DisplayName;
            fieldReport.ID = field.Item.ID;
            fieldReport.Path = field.Item.Paths.FullPath;
            fieldReport.FieldName = field.Name;
		    fieldReport.InstancesReplaced = 0;
	        fieldReport.Language = field.Language.Name;
            if (!preview) field.Item.Editing.BeginEdit();
		    foreach (var tokenLocation in TokenKeeper.CurrentKeeper.ParseTokenLocations(field))
		    {
		        var tokenIdentifier = field.Value.Substring(tokenLocation.Item1, tokenLocation.Item2);
		        var tokenProps = TokenKeeper.CurrentKeeper.TokenProperties(tokenIdentifier);
		        if (tokenProps["Category"] != _category || tokenProps["Token"] != _token)
		            continue;
		        fieldReport.InstancesReplaced++;
		        if (!preview)
		        {
                    StringBuilder sb = new StringBuilder(field.Value);
		            sb.Remove(tokenLocation.Item1, tokenLocation.Item2);
		            if (_replaceWithValue)
		            {
                        tokenProps.Add("Language", field.Language.Name);
		                sb.Insert(tokenLocation.Item1, TokenKeeper.CurrentKeeper.GetTokenValue(_category, _token, tokenProps));
		            }
		            field.Value = sb.ToString();
		        }
		    }
            if (!preview) field.Item.Editing.EndEdit();
            ret.Converted.Add(fieldReport);
		}
	}
}
