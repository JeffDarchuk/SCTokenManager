using System.Runtime.InteropServices.WindowsRuntime;
using TokenManager.Data.Interfaces;

namespace TokenManager.Management
{


	public class TokenKeeper
	{
		internal static ITokenKeeperService TokenSingleton;
		internal static bool _isSc8 = false;
		public static ITokenKeeperService CurrentKeeper { get { return TokenSingleton; } }
		public static bool IsSc8 => _isSc8;
	}
}
