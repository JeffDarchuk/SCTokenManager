using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Query;
using TokenManager.Data.TokenDataTypes.Attributes;

namespace TokenManager
{
	public class DemoTokenFullModel
	{
		[TokenString(label: "Give us a string",placeholder: "Enter string here", required: true)]
		public string MyString;

		[TokenInteger(label: "Give us an int", required: true)]
		public int MyInt;

		[TokenBoolean(label: "Give us a boolean")]
		public bool MyBoolean;

		[TokenId(label: "Give us an item", required: true)]
		public ID MyItem;

		[TokenDroplist(label: "Give us an option", required: true, options: new[] { "op1", "Option 1", "op2", "Option 2" })]
		public string MyDroplist;

		// Options will be the children of the item at the id given.  The value will be the item name, or the item id depending on the valueAsId boolean.
		[TokenDroplist(label: "Give us an option based on the sitecore items at this root", required: true, id:"{62BC5816-165B-413D-B8E7-692063D12374}", valueAsId:false)]
		public string MyDroplist2;

		[TokenGeneralLink(label: "Give us a link", required: true, root:"{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}")]
		public string MyLink;
		
	}
}
