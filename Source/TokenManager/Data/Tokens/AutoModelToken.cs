using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.Tokens
{
	public abstract class AutoToken<T> : AutoToken
		where T : class, new()
	{
		public AutoToken(string collectionName, string tokenIcon, string tokenName) : base(collectionName, tokenIcon, tokenName)
		{
		}

		public abstract string Render(T model);
		public virtual T GenerateModel(TokenDataCollection extraData)
		{
			T model = (T)Activator.CreateInstance(typeof(T));
			foreach (var prop in typeof(T).GetProperties())
			{
				foreach (var attr in prop.GetCustomAttributes(typeof(ITokenDataAttribute), false).OfType<ITokenDataAttribute>())
				{
					prop.SetValue(model, attr.GetObject(extraData, prop.Name, prop.PropertyType));
				}
			}
			foreach (var field in typeof(T).GetFields())
			{
				foreach (var attr in field.GetCustomAttributes(typeof(ITokenDataAttribute), false).OfType<ITokenDataAttribute>())
				{
					field.SetValue(model, attr.GetObject(extraData, field.Name, field.FieldType));
				}
			}
			return model;
		}
		public override IEnumerable<ITokenData> ExtraData()
		{
			foreach (var prop in typeof(T).GetProperties())
			{
				foreach (var attr in prop.GetCustomAttributes(typeof(ITokenDataAttribute), false).OfType<ITokenDataAttribute>())
				{
					ITokenData ret = attr.TokenData;
					ret.Name = prop.Name;
					yield return ret;
				}
			}
			foreach (var field in typeof(T).GetFields())
			{
				foreach (var attr in field.GetCustomAttributes(typeof(ITokenDataAttribute), false).OfType<ITokenDataAttribute>())
				{
					ITokenData ret = attr.TokenData;
					ret.Name = field.Name;
					yield return ret;
				}
			}
		}
		/// <summary>
		/// Specifies the way the token is rendered in the RTE.  Beware of invalid markup, everything entered here needs to be valid markup for a p tag.  Otherwise the RTE may eat it.
		/// </summary>
		/// <param name="model">model for token</param>
		/// <returns>Markup for the token</returns>
		public virtual string TokenIdentifierText(T model)
		{
			return "";
		}
		/// <summary>
		/// Specifies the style to be applied to the token.
		/// </summary>
		/// <param name="model"></param>
		/// <returns>Inline style string for the token identifier</returns>
		public virtual string TokenIdentifierStyle(T model)
		{
			return "";
		}
		public sealed override string TokenIdentifierStyle(TokenDataCollection extraData)
		{
			string modelStyle = TokenIdentifierStyle(GenerateModel(extraData));
			if (!string.IsNullOrWhiteSpace(modelStyle))
				return modelStyle;
			return base.TokenIdentifierStyle(extraData);
		}

		public sealed override string Value(TokenDataCollection extraData)
		{
			return Render(GenerateModel(extraData));
		}
		public sealed override string TokenIdentifierText(TokenDataCollection extraData)
		{
			string modelIdentifier = TokenIdentifierText(GenerateModel(extraData));
			if (!string.IsNullOrWhiteSpace(modelIdentifier))
				return modelIdentifier;
			return base.TokenIdentifierText(extraData);
		}
	}
}
