using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.TokenDataTypes;
using TokenManager.Data.TokenDataTypes.Attributes;

namespace TokenManagerDemo.YouTube
{
	public class YouTubeModel
	{
		[TokenString("YouTube video id", "DFStzMQY_nc", true, "DFStzMQY_nc")]
		public string VideoId;
	}
}
