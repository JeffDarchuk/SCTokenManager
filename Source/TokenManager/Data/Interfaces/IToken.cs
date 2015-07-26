using Sitecore.Data;

namespace TokenManager.Data.Interfaces
{
	public interface IToken
	{
		string Token { get; }
		string Value {get; }
		ID GetBackingItemId();
	}
}
