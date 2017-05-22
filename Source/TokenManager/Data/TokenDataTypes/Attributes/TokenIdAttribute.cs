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
	public class TokenIdAttribute : Attribute, ITokenDataAttribute
	{
		private IdTokenData Data;
		/// <summary>
		/// Marks this property or field as being a sitecore item id token value, this is applicable to a ID, Item or string type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="required">Is this value required or not</param>
		/// <param name="root">The Sitecore ID that will be used for the internal link root</param>
		public TokenIdAttribute(string label, bool required, string root)
		{
			Data = new IdTokenData(label, "", required, root);
		}
		/// <summary>
		/// Marks this property or field as being a sitecore item id token value, this is applicable to a ID, Item or string type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="required">Is this value required or not</param>
		public TokenIdAttribute(string label, bool required)
		{
			Data = new IdTokenData(label, "", required);
		}
		public ITokenData TokenData => Data;

		public virtual object GetObject(TokenDataCollection collection, string name, Type t)
		{
			if (t == typeof(string))
				return collection.GetString(name);
			if (t == typeof(Item))
				return (Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database ?? Factory.GetDatabase("master")).GetItem(collection.GetString(name));
			if (t == typeof(ID))
				return new ID(collection.GetString(name));
			throw new TokenCastException($"Unable to cast type {t.Namespace}.{t.Name} for TokenGeneralLinkAttribute on property/field {name}.  Acceptable types are ID, Item or string.");

		}
	}
}
