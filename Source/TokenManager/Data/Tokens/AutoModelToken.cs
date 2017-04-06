using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.Tokens
{
	public abstract class ModelAutoToken<T> : AutoToken
		where T : class, new()
	{
		public ModelAutoToken(string collectionName, string tokenIcon, string tokenName) : base(collectionName, tokenIcon, tokenName)
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

		public override string Value(TokenDataCollection extraData)
		{
			return Render(GenerateModel(extraData));
		}
	}
}
