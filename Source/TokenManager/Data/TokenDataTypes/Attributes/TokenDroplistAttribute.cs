using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.TokenDataTypes.Attributes
{
	public class TokenDroplistAttribute : Attribute, ITokenDataAttribute
	{
		private DroplistTokenData Data;

		/// <summary>
		/// Marks this property or field as being a droplist token value, this is applicable to a string, ID, or Item type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="required">Is this value required or not</param>
		/// <param name="options">Array of options that correspond to key/value pairs, each odd index is the key which is used as the label and the following even index is the value which is the hidden value that will be passed to your model</param>
		/// <param name="defaultValue">The starting value of the token data</param>
		public TokenDroplistAttribute(string label, bool required, string[] options, string defaultValue = "")
		{
			List<KeyValuePair<string, string>> ops = new List<KeyValuePair<string, string>>();
			for (int i = 0; i + 1 < options.Length; i += 2)
			{
				ops.Add(new KeyValuePair<string, string>(options[i], options[i + 1]));
			}
			Data = new DroplistTokenData(label, "", required, ops, defaultValue);
		}

		/// <summary>
		/// Marks this property or field as being a droplist token value, this is applicable to a string, ID, or Item type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="required">Is this value required or not</param>
		/// <param name="id">Sitecore ID for which the child elements will be made into the options</param>
		/// <param name="valueAsId">True: items Sitecore id False: items name</param>
		/// <param name="defaultValue">The starting value of the token data</param>
		public TokenDroplistAttribute(string label, bool required, string id, bool valueAsId = true, string defaultValue = "")
		{
			List<KeyValuePair<string, string>> ops = new List<KeyValuePair<string, string>>();
			Item item = (Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database ?? Factory.GetDatabase("master")).GetItem(id);
			if (item != null)
				foreach (Item child in item.Children)
				{
					ops.Add(new KeyValuePair<string, string>(child.DisplayName, valueAsId ? child.ID.ToString() : child.Name));
				}
			Data = new DroplistTokenData(label, "", required, ops, defaultValue);
		}

		public ITokenData TokenData => Data;

		public virtual object GetObject(TokenDataCollection collection, string name, Type t)
		{
			if (t == typeof(string))
				return collection.GetString(name);
			if (t == typeof(Item))
				return (Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database ?? Factory.GetDatabase("master")).GetItem(collection.GetId(name) ?? ID.Null);
			if (t == typeof(ID))
				return collection.GetId(name);
			throw new TokenCastException($"Unable to cast type {t.Namespace}.{t.Name} for TokenDroplistAttribute on property/field {name}.  Acceptable type is string.");

		}
	}
}
