using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.SecurityModel;
using TokenManager.Collections;
using TokenManager.Data.Interfaces;
using TokenManager.Management;

namespace TokenManager.Handlers.TokenOperations
{
	/// <summary>
	/// changes given text into tokens
	/// </summary>
	class TokenIncorporator
	{
		private readonly string _root;
		private readonly Database _database;
		private readonly Item _item;
		private readonly string _category;
		private readonly string _tokenName;
		private string _tokenValue;
		private ITokenCollection<IToken> _tokenCollection;
		private readonly string _tokenIdentifier;
		public ITokenCollection<IToken> TokenCollection
		{
			get
			{
				if (_tokenCollection != null)
					return _tokenCollection;
				_tokenCollection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(_category);
				return _tokenCollection;
			}
		}
		/// <summary>
		/// set up the incorporator to use the token idenfier values and a value
		/// </summary>
		/// <param name="category"></param>
		/// <param name="tokenName"></param>
		/// <param name="tokenValue"></param>
		public TokenIncorporator(string root, string category, string tokenName, string tokenValue)
		{
			_root = root;
			_database = TokenKeeper.CurrentKeeper.GetDatabase();
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
		public dynamic Incorporate(bool preview = false, string type = "plain")
		{
			if (type == "plain")
				_tokenValue = Regex.Escape(_tokenValue);
			dynamic ret = new ExpandoObject();
			ret.Count = 0;
			ret.Converted = new List<ExpandoObject>();
			using (new SecurityDisabler())
			{
				if (!preview && !TokenCollection.HasToken(_tokenName) && TokenCollection is SitecoreTokenCollection<IToken>)
				{
					Item newToken = _item.Add(_tokenName, new TemplateID(new ID("{87BFAA2C-2E2F-42C6-A135-9F2AE7D32807}")));
					newToken.Editing.BeginEdit();
					newToken["Token"] = _tokenName;
					newToken["Value"] = _tokenValue;
					newToken.Editing.EndEdit();
					var collection = TokenKeeper.CurrentKeeper.GetTokenCollection<IToken>(_category) as TokenCollection<IToken>;
					if (collection != null)
						collection.AddOrUpdateToken(collection.InitiateToken(_tokenName));
				}
				Stack<Item> itemStack = new Stack<Item>();
				Item content = _database.GetItem(new ID(_root));
				LoadAllLanguageItems(content, itemStack);
				{
					while (itemStack.Any())
					{
						var cur = itemStack.Pop();
						if (cur.IsTokenManagerItem() || !_tokenCollection.IsCurrentContextValid(cur))
							continue;
						foreach (var field in cur.Fields.Where(f => f.Type == "Rich Text"))
						{
							Regex r = new Regex(_tokenValue);
							MatchCollection m = r.Matches(field.Value);
							if (m.Count > 0)
							{
								var ttt = ConvertTextToToken(field, m, preview);
								if (ttt != null)
								{
									ret.Count++;

									ret.Converted.Add(ttt);
								}
							}
						}
						if (LanguageManager.DefaultLanguage.Name == cur.Language.Name)
							foreach (Item child in cur.Children.Where(c => !TemplateManager.GetTemplate(c).IsDerived(new ID(Constants.TokenTemplateBaseId))))
								LoadAllLanguageItems(child, itemStack);
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// tracks down the text in the rich text field and transforms it into a token
		/// </summary>
		/// <param name="field"></param>
		/// <param name="match"></param>
		/// <param name="regex"></param>
		/// <param name="preview"></param>
		/// <returns>dynamic object related to how the field changed</returns>
		private dynamic ConvertTextToToken(Field field, MatchCollection matches, bool preview)
		{
			dynamic ret = new ExpandoObject();
			ret.InstancesReplaced = 0;
			var sb = new StringBuilder(field.Value);
			for (int i = matches.Count - 1; i >= 0; i--)
			{
				var match = matches[i];
				var group = match.Groups[0];
				if (!TokenKeeper.CurrentKeeper.IsInToken(field, group.Index, group.Length))
				{
					ret.InstancesReplaced++;
					sb.Remove(group.Index, group.Length);
					sb.Insert(group.Index, _tokenIdentifier);
				}
			}
			if (ret.InstancesReplaced == 0)
				return null;
			ret.DisplayName = field.Item.DisplayName;
			ret.ID = field.Item.ID;
			ret.FieldName = field.Name;
			ret.Path = field.Item.Paths.FullPath;
			ret.Language = field.Language.Name;
			if (!preview)
			{
				field.Item.Editing.BeginEdit();
				field.Value = sb.ToString();
				field.Item.Editing.EndEdit();
			}
			return ret;
		}

		/// <summary>
		/// constructs the token identifier for the current context
		/// </summary>
		/// <returns></returns>
		private string GetTokenString()
		{
			return TokenKeeper.CurrentKeeper.GetTokenIdentifier(_category, _tokenName, (dynamic)null);
		}

		private void LoadAllLanguageItems(Item master, Stack<Item> itemStack)
		{
			foreach (Item i in ItemManager.GetContentLanguages(master).Select(l => master.Database.GetItem(master.ID, l)))
			{
				itemStack.Push(i);
			}
		}
	}
}
