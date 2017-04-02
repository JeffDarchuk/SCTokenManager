using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.Data.TokenExtensions
{
	public class TokenButton
	{
		public TokenButton()
		{
		}
		public TokenButton(string name, string icon, int sortOrder)
		{
			Name = name;
			Icon = icon;
			SortOrder = sortOrder;
		}
		public string Name;
		public string Icon;
		public int SortOrder = 1125;
	}
}
