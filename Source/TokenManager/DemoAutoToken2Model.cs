using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.TokenDataTypes.Attributes;

namespace TokenManager
{
	public class DemoAutoToken2Model
	{
		//Indicates the user should be prompted to enter a string value when inserting this token.
		[TokenString(label: "Give us a string", placeholder: "Enter string here", required: true)]
		public string MySecretValue { get; set; }
		public string UserName { get; set; }
	}
}
