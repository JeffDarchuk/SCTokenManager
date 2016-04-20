using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.ContentSearch.Linq.Nodes;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace TokenManager.Data.Interfaces
{
	public interface ITokenIdentifier
	{

		void AddType(XmlNode node);
		T ResolveCollection<T>(Item item)
			where T : ITokenCollection<IToken>;

		T ResolveToken<T>(ITokenCollection<IToken> collection, string tokenName)
			where T : IToken;

		IEnumerable<ID> GetAllTokenTemplates();
	}
}
