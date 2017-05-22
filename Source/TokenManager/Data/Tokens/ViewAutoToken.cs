using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.Tokens
{
	public abstract class ViewAutoToken<T> : AutoToken<T>
		where T : class, new()
	{
		protected ViewAutoToken(string collectionName, string tokenIcon, string tokenName) : base(collectionName, tokenIcon, tokenName)
		{
		}
		public abstract string GetViewPath(T model);
		public override string Render(T model)
		{
			return ViewRenderer.RenderPartialView(GetViewPath(model), model);
		}
	}
}
