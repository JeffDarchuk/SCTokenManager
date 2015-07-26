using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.SecurityModel;

using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Handlers
{
	/// <summary>
	/// changes given text into tokens
	/// </summary>
	class TokenIncorporator
	{
		private const string _contentFolderGuid = "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}";
		private readonly Database _database;
		private Item _item;
		private string _category;
		private string _tokenName;
		private string _tokenValue;
		private ITokenCollection _tokenCollection;
		private string _tokenIdentifier;
		public ITokenCollection TokenCollection
		{
			get
			{
				if (_tokenCollection != null)
					return _tokenCollection;
				_tokenCollection = TokenKeeper.CurrentKeeper.GetTokenCollection(_category);
				return _tokenCollection;
			}
		}
		/// <summary>
		/// set up the incorporator to use the token idenfier values and a value
		/// </summary>
		/// <param name="category"></param>
		/// <param name="tokenName"></param>
		/// <param name="tokenValue"></param>
		public TokenIncorporator(string category, string tokenName, string tokenValue)
		{
			_database = Database.GetDatabase("master");
			_category = category;
			_item = _database.GetItem(TokenCollection.GetBackingItemId());
			_tokenName = tokenName;
			_tokenValue = tokenValue;
			_tokenIdentifier = GetTokenString();
		}

		/// <summary>
		/// inject token over the given token values found in rich text fields
		/// </summary>
		/// <returns>dynamic object with information about what happened</returns>
		public dynamic Incorporate()
		{
			dynamic ret = new ExpandoObject();
			ret.Count = 0;
			ret.Converted = new List<ExpandoObject>();
			using (new SecurityDisabler())
			{
				if (!TokenCollection.HasToken(_tokenName) && TokenCollection is SitecoreTokenCollection)
				{
					Item newToken = _item.Add(_tokenName, new TemplateID(new ID("{87BFAA2C-2E2F-42C6-A135-9F2AE7D32807}")));
					newToken.Editing.BeginEdit();
					newToken["Token"] = _tokenName;
					newToken["Value"] = _tokenValue;
					newToken.Editing.EndEdit();
					var collection = TokenKeeper.CurrentKeeper.GetTokenCollection(_category) as TokenCollection<IToken>;
					if (collection != null)
						collection.AddOrUpdateToken(collection.InitiateToken(_tokenName));
				}
				Stack<Item> itemStack = new Stack<Item>();
				Item content = _database.GetItem(new ID(_contentFolderGuid));
				itemStack.Push(content);

				{
					while (itemStack.Any())
					{
						var cur = itemStack.Pop();
						foreach (var field in cur.Fields.Where(f => f.Type == "Rich Text" && f.Value.Contains(_tokenValue)))
						{
							ret.Count++;
							ret.Converted.Add(ConvertTextToToken(field));
						}
						foreach (Item child in cur.Children.Where(c=>!TemplateManager.GetTemplate(c).IsDerived(new ID(Constants._tokenTemplateBaseId))))
							itemStack.Push(child);
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// tracks down the text in the rich text field and transforms it into a token
		/// </summary>
		/// <param name="field"></param>
		/// <returns>dynamic object related to how the field changed</returns>
		private dynamic ConvertTextToToken(Field field)
		{
			dynamic ret = new ExpandoObject();
			ret.DisplayName = field.Item.DisplayName;
			ret.ID = field.Item.ID;
			ret.FieldName = field.Name;
			ret.Path = field.Item.Paths.FullPath;
			var parts = field.Value.Split(new[] { _tokenValue }, StringSplitOptions.None);
			ret.InstancesReplaced = parts.Length - 1;
			field.Item.Editing.BeginEdit();
			field.Value = string.Join(_tokenIdentifier, parts);
			field.Item.Editing.EndEdit();
			return ret;
		}

		/// <summary>
		/// constructs the token identifier for the current context
		/// </summary>
		/// <returns></returns>
		private string GetTokenString()
		{
			return TokenKeeper.CurrentKeeper.TokenPrefix + TokenCollection.GetCollectionLabel() +
			       TokenKeeper.CurrentKeeper.Delimiter + _tokenName + TokenKeeper.CurrentKeeper.TokenSuffix;
		}
	}
}
