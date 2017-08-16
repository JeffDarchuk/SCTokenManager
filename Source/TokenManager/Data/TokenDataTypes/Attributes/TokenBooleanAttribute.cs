using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.TokenDataTypes.Attributes
{
	public class TokenBooleanAttribute : Attribute, ITokenDataAttribute
	{
		private BooleanTokenData Data;

		/// <summary>
		/// Marks this property or field as being a checkbox token value, this is applicable to a boolean, int, or string type.
		/// </summary>
		/// <param name="label">The description given to content authors filling in the field</param>
		/// <param name="defaultValue">The starting value of the token data</param>
		public TokenBooleanAttribute(string label, bool defaultValue = false)
		{
			Data = new BooleanTokenData(label, "", defaultValue);
		}
		public ITokenData TokenData => Data;

		public virtual object GetObject(TokenDataCollection collection, string name, Type t)
		{
			if (t == typeof(bool))
				return collection.GetBoolean(name);
			if (t == typeof(string))
				return collection.GetString(name);
			if (t == typeof(int))
				return collection.GetString(name) == "True" ? 1 : 0;
			throw new TokenCastException($"Unable to cast type {t.Namespace}.{t.Name} for TokenBooleanAttribute on property/field {name}.  Acceptable types are bool, string, int.");
		}
	}
}
