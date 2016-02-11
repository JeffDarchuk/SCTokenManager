using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;

namespace TokenManager.Management
{
	public class DefaultTokenIdentifier : ITokenIdentifier
	{
		public delegate T ObjectActivator<out T>(params object[] args);
		Dictionary<string, XmlNode> templateToType = new Dictionary<string, XmlNode>();
		public void AddType(XmlNode node)
		{
			if (node.Attributes != null) templateToType[node.Attributes["templateId"].Value] = node;
		}

		public virtual T ResolveCollection<T>(Item item)
			where T : ITokenCollection<IToken>
		{
			var tid = item.TemplateID.ToString();
			if (templateToType.ContainsKey(tid))
			{
				var t = Factory.CreateType(templateToType[tid], true);
				ConstructorInfo ci = t.GetConstructors().First();

				ObjectActivator<T> activator = GetActivator<T>(ci);
				return activator(new object[] { item, item.TemplateID });
			}
			return default(T);
		}

		public virtual T ResolveToken<T>(ITokenCollection<IToken> collection, string tokenName) where T : IToken
		{
			var item = TokenKeeper.CurrentKeeper.GetDatabase().GetItem(collection.GetBackingItemId());
			var tokenItem = item.Children.FirstOrDefault(x => x["Token"] == tokenName);

			if (tokenItem != null)
			{
				var tid = tokenItem.TemplateID.ToString();
				if (templateToType.ContainsKey(tid))
				{
					var t = Factory.CreateType(templateToType[tid], false);
					ConstructorInfo ci = t.GetConstructors().First();
					ObjectActivator<T> activator = GetActivator<T>(ci);
					return activator(new object[] { tokenName, tokenItem.ID });
				}
			}
			return default(T);
		}

		/// <summary>
		/// from http://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ctor"></param>
		/// <returns></returns>
		public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
		{
			Type type = ctor.DeclaringType;
			ParameterInfo[] paramsInfo = ctor.GetParameters();

			//create a single param of type object[]
			ParameterExpression param =
				Expression.Parameter(typeof(object[]), "args");

			Expression[] argsExp =
				new Expression[paramsInfo.Length];

			//pick each arg from the params array 
			//and create a typed expression of them
			for (int i = 0; i < paramsInfo.Length; i++)
			{
				Expression index = Expression.Constant(i);
				Type paramType = paramsInfo[i].ParameterType;

				Expression paramAccessorExp =
					Expression.ArrayIndex(param, index);

				Expression paramCastExp =
					Expression.Convert(paramAccessorExp, paramType);

				argsExp[i] = paramCastExp;
			}

			//make a NewExpression that calls the
			//ctor with the args we just created
			NewExpression newExp = Expression.New(ctor, argsExp);

			//create a lambda with the New
			//Expression as body and our param object[] as arg
			LambdaExpression lambda =
				Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

			//compile it
			ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
			return compiled;
		}
	}
}
