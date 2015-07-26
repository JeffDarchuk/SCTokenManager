using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

using TokenManager.ContentSearch;
using TokenManager.Management;

namespace TokenManager.Handlers
{
	/// <summary>
	/// unzipping here referrs to the process of replacing all token identifiers with their value
	/// thus effictively "untokening" the token
	/// </summary>
	class TokenUnzipper
	{
		private readonly string _category;
		private readonly string _token;
		private readonly Database _database;

		/// <summary>
		/// constructs an unzipper for the specified token identifying information
		/// </summary>
		/// <param name="category"></param>
		/// <param name="token"></param>
		public TokenUnzipper(string category, string token)
		{
			_category = category;
			_token = token;
			_database = Database.GetDatabase("master");
		}

		/// <summary>
		/// removes all instances of this token, replacing them with the token value
		/// </summary>
		/// <returns>dynamic object with statistics of what transpired</returns>
		public dynamic Unzip()
		{
			dynamic ret = new ExpandoObject();
			ret.Count = 0;
			ret.Converted = new List<ExpandoObject>();
			using (new SecurityDisabler())
			{
				foreach( ContentSearchTokens current in TokenKeeper.CurrentKeeper.GetTokenOccurances(_category, _token))
				{
					Item item = _database.GetItem(ID.Parse(current.Id));
					foreach (var field in item.Fields.Where(f => f.Type == "Rich Text"))
						ProcessRTE(field, ret);
				}
			}
			return ret;
		}

		/// <summary>
		/// process this rich text field looking for tokens to wipe out
		/// </summary>
		/// <param name="field"></param>
		/// <param name="ret"></param>
		private void ProcessRTE(Field field, dynamic ret)
		{
			var tokenIdentifier = TokenKeeper.CurrentKeeper.GetTokenIdentifier(_category, _token);
			if (field.Value.LastIndexOf(tokenIdentifier, StringComparison.Ordinal) == -1) return;
			ret.Count++;
			dynamic fieldReport = new ExpandoObject();
			fieldReport.DisplayName = field.Item.DisplayName;
			fieldReport.ID = field.Item.ID;
			fieldReport.Path = field.Item.Paths.FullPath;
			fieldReport.FieldName = field.Name;
			var parts = field.Value.Split(new[] {tokenIdentifier}, StringSplitOptions.None);
			fieldReport.InstancesReplaced = parts.Length - 1;
			field.Item.Editing.BeginEdit();
			field.Value = string.Join(TokenKeeper.TokenSingleton.GetTokenValue(_category, _token), parts);
			field.Item.Editing.EndEdit();
			ret.Converted.Add(fieldReport);
		}
	}
}
