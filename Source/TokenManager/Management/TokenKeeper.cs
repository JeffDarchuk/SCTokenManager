using TokenManager.Data.Interfaces;

namespace TokenManager.Management
{


	public class TokenKeeper
	{
		internal static ITokenKeeperService TokenSingleton;
		public static ITokenKeeperService CurrentKeeper { get { return TokenSingleton; } }

	}
}
