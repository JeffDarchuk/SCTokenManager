using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

using TokenManager.Management;

namespace TokenManager.Handlers
{
	/// <summary>
	/// takes an abandoned tokens from a former identifier scheme and incorporates them
	/// </summary>
	public class TokenConverter
	{
		private readonly string _prefix;
		private readonly string _suffix;
		private readonly string _delimiter;
		private const string _contentFolderGuid = "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}";

		/// <summary>
		/// sets up the converter for the previous token identifiers
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="suffix"></param>
		/// <param name="delimiter"></param>
		public TokenConverter(string prefix, string suffix, string delimiter)
		{
			_prefix = HttpUtility.HtmlEncode(prefix);
			_suffix = HttpUtility.HtmlEncode(suffix);
			_delimiter = HttpUtility.HtmlEncode(delimiter);
		}

		/// <summary>
		/// converts all old tokens to the new format
		/// </summary>
		/// <returns>dynamic object with information about what happened here</returns>
		public dynamic Convert()
		{
			dynamic ret = new ExpandoObject();
			ret.Count = 0;
			ret.Items = new List<ExpandoObject>(); 
			using (new SecurityDisabler())
			{
				foreach (var field in GetFieldsWithOldTokens())
				{
					var convertedTokenValue = ConvertTokens(field.Value);
					if (convertedTokenValue == field.Value) continue;
					ret.Count++;
					dynamic item = new ExpandoObject();
					item.Id = field.Item.ID;
					item.DisplayName = field.Item.DisplayName;
					item.Path = field.Item.Paths.FullPath;
					item.Database = field.Database.Name;
					item.Field = field.Name;

					field.Item.Editing.BeginEdit();
					field.Value = convertedTokenValue;
					field.Item.Editing.EndEdit();

					ret.Items.Add(item);
				}
			}
			ret.Response = "Success";
			return ret;
		}
		/// <summary>
		/// convert tokens found in the text
		/// </summary>
		/// <param name="text"></param>
		/// <returns>text with tokens converted</returns>
		private string ConvertTokens(string text)
		{
			var sb = new StringBuilder(text);
			int pi = text.LastIndexOf(_prefix, StringComparison.Ordinal);
			int si = text.Length;
			int di = text.Length;
			while (pi != -1)
			{
				si = text.IndexOf(_suffix, pi, si - pi, StringComparison.Ordinal);
				if (si > -1)
					di = text.IndexOf(_delimiter, pi, si - pi, StringComparison.Ordinal);
				if (si > -1 && di > -1 && si > di)
				{
					sb.Replace(_suffix, TokenKeeper.CurrentKeeper.TokenSuffix, si, _suffix.Length);
					sb.Replace(_delimiter, TokenKeeper.CurrentKeeper.Delimiter, di, _delimiter.Length);
					sb.Replace(_prefix, TokenKeeper.CurrentKeeper.TokenPrefix, pi, _prefix.Length);
				}
				else
				{
					si = pi;
					di = pi;
				}
				pi = text.LastIndexOf(_prefix, pi - _suffix.Length, StringComparison.Ordinal);
			}
			return sb.ToString();
		}

		/// <summary>
		/// find fields that contain the old tokens
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Field> GetFieldsWithOldTokens()
		{
			foreach (var db in Factory.GetDatabases())
			{
				Stack<Item> itemStack = new Stack<Item>();
				Item content = db.GetItem(new ID(_contentFolderGuid));
				if (content == null) continue;
				itemStack.Push(content);
				while (itemStack.Any())
				{
					var cur = itemStack.Pop();
					foreach (var field in cur.Fields.Where(f => f.Type == "Rich Text" && HasOldTokens(f.Value)))
					{
						yield return field;
					}
					foreach (Item child in cur.Children)
						itemStack.Push(child);
				}
			}
		}

		/// <summary>
		/// returns if the text has the parts for the old token structure
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private bool HasOldTokens(string p)
		{
			return new[] { _prefix, _delimiter, _suffix }.All(p.Contains);
		}
	}
}
