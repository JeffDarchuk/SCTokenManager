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
	public class TokenStringAttribute : Attribute, ITokenDataAttribute
	{
		private StringTokenData Data;

		/// <summary>
		/// Marks this property or field as being a string token value, this is applicable to a string type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="placeholder">The example text that shadows in the textbox</param>
		/// <param name="required">Is this value required or not</param>
		public TokenStringAttribute(string label, string placeholder,  bool required)
		{
			Data = new StringTokenData(label, "", placeholder, required);
		}
		public ITokenData TokenData => Data;

		public virtual object GetObject(TokenDataCollection collection, string name, Type t)
		{
			if (t == typeof(string))
				return collection.GetString(name);

			throw new TokenCastException($"Unable to cast type {t.Namespace}.{t.Name} for TokenGeneralLinkAttribute on property/field {name}.  Acceptable types are ID, Item or string.");

		}
	}
}
