using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Tokens;

namespace TokenManagerDemo.Map
{
	public class MapToken : ViewAutoToken<MapModel>
	{
		public MapToken() : base("Google", "Office/32x32/earth_location.png", "Map")
		{
		}

		public override string GetViewPath(MapModel model)
		{
			return "/views/tokens/MapToken.cshtml";
		}
	}
}
