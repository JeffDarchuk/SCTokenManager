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
	public class TokenIntegerAttribute : Attribute, ITokenDataAttribute
	{
		private IntegerTokenData Data;

		/// <summary>
		/// Marks this property or field as being a general link token value, this is applicable to a int or string type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="required">Is this value required or not</param>
		public TokenIntegerAttribute(string label, bool required)
		{
			Data = new IntegerTokenData(label, "", required);
		}
		public ITokenData TokenData => Data;

		public virtual object GetObject(TokenDataCollection collection, string name, Type t)
		{
			if (t == typeof(string))
				return collection.GetString(name);
			if (t == typeof(int))
				return collection.GetInt(name);

			throw new TokenCastException($"Unable to cast type {t.Namespace}.{t.Name} for TokenGeneralLinkAttribute on property/field {name}.  Acceptable types are ID, Item or string.");

		}
	}
}
