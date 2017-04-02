using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.Tokens
{
	public abstract class ViewAutoToken : AutoToken
	{
		public ViewAutoToken(string collectionName, string tokenIcon, string tokenName) : base(collectionName, tokenIcon, tokenName)
		{
		}
		public abstract object GetModel(TokenDataCollection extraData);
		public abstract string GetViewPath(TokenDataCollection extraData);
		public override string Value(TokenDataCollection extraData)
		{
			return ViewRenderer.RenderPartialView(GetViewPath(extraData), GetModel(extraData));
		}
	}
}
