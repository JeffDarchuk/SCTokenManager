using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.TokenDataTypes.Attributes;

namespace TokenManagerDemo.Map
{
	public class MapModel
	{
		[TokenString("Address", "Camas, WA", true, "Camas, WA")]
		public string Address;
	}
}
