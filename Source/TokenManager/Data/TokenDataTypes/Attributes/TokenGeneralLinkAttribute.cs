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
	/// <summary>
	/// Attribute to bind a link token data to a TokenManager.Data.TokenDataTypes.Support.GeneralLink or string
	/// </summary>
	public class TokenGeneralLinkAttribute : Attribute, ITokenDataAttribute
	{
		private GeneralLinkTokenData Data;
		/// <summary>
		/// Marks this property or field as being a general link token value, this is applicable to a TokenManager.Data.TokenDataTypes.Support.GeneralLink, ID, Item or string type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="required">Is this value required or not</param>
		/// <param name="root">The Sitecore ID that will be used for the internal link root</param>
		public TokenGeneralLinkAttribute(string label, bool required, string root)
		{
			Data = new GeneralLinkTokenData(label, "", required, root);
		}
		/// <summary>
		/// Marks this property or field as being a general link token value, this is applicable to a TokenManager.Data.TokenDataTypes.Support.GeneralLink, ID, Item or string type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="required">Is this value required or not</param>
		public TokenGeneralLinkAttribute(string label, bool required)
		{
			Data = new GeneralLinkTokenData(label, "", required);
		}
		public ITokenData TokenData => Data;

		public virtual object GetObject(TokenDataCollection collection, string name, Type t)
		{
			if (t == typeof(GeneralLink))
				return collection.GetLink(name);
			if (t == typeof(string))
				return collection.GetLink(name).Href;
			if (t == typeof(Item))
				return (Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database ?? Factory.GetDatabase("master")).GetItem(collection.GetLink(name).InternalLinkId);
			if (t == typeof(ID))
				return new ID(collection.GetLink(name).InternalLinkId);
			throw new TokenCastException($"Unable to cast type {t.Namespace}.{t.Name} for TokenGeneralLinkAttribute on property/field {name}.  Acceptable types are TokenManager.Data.TokenDataTypes.Support.GeneralLink, string.");

		}
	}
}
