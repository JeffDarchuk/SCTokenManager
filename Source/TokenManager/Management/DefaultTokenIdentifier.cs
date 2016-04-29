using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;

namespace TokenManager.Management
{
	public class DefaultTokenIdentifier : ITokenIdentifier
	{
		public delegate T ObjectActivator<out T>(params object[] args);
		private Dictionary<string, XmlNode> templateToType = new Dictionary<string, XmlNode>();
		private List<ID> tokenIds = new IdList(); 
		public void AddType(XmlNode node)
		{
			if (node?.Attributes != null && (node.ParentNode == null || node.Attributes["templateId"] == null))
				throw new ArgumentException("The xml structure is not set up correctly, ensure that the token node has a templateId attribute");
			if (node?.ParentNode != null && (node.ParentNode.Name == "tokens"))
				if (node.Attributes != null) tokenIds.Add(new ID(node.Attributes["templateId"].Value));
			if (node?.Attributes != null) templateToType[node.Attributes["templateId"].Value] = node;
		}

		public virtual T ResolveCollection<T>(Item item)
			where T : ITokenCollection<IToken>
		{
			var tid = item.TemplateID.ToString();
			if (templateToType.ContainsKey(tid))
			{
				var t = Factory.CreateType(templateToType[tid], true);
				ConstructorInfo ci = t.GetConstructors().FirstOrDefault(x=>x.GetParameters().Length==1 && x.GetParameters()[0].ParameterType == typeof(Item));
				if (ci == null) throw new ArgumentException("While trying to initialize token collection type "+t.Name+ " no appropriate constructor was found, it should take one parameter of type Item");
				return (T)ci.Invoke(new object[] { item });
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
					Type t = Factory.CreateType(templateToType[tid], false);
					ConstructorInfo ci = t.GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(string) && x.GetParameters()[1].ParameterType == typeof(ID));
					if (ci == null) throw new ArgumentException("While trying to initialize token type " + t.Name + " no appropriate constructor was found, it should take two parameters of type String (name) and ID (token sitecore Item Id)");
					return (T)ci.Invoke(new object[] {tokenName, tokenItem.ID});
				}
			}
			return default(T);
		}

		public IEnumerable<ID> GetAllTokenTemplates()
		{
			return tokenIds;
		}
	}
}
