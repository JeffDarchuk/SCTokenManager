using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;

namespace TokenManager.Data.TokenDataTypes.Support
{
	public class GeneralLink
	{
		private readonly dynamic _jsFields = new ExpandoObject();

		public IDictionary<string, object> JsFields => _jsFields as IDictionary<string, object>;

		public bool Internal
		{
			get
			{
				if (!JsFields.ContainsKey("type"))
					throw new ArgumentException("Malformed token manager general link data doesn't contain the field type");
				return JsFields["type"].ToString() == "internal";
			}
		}

		public string ExternalUrl
		{
			get
			{
				if (!JsFields.ContainsKey("url"))
					return "";
				return JsFields["url"].ToString();
			}
		}

		public string InternalLinkId
		{
			get
			{
				if (!JsFields.ContainsKey("id"))
					return "";
				return JsFields["id"].ToString();
			}
		}
		public string LinkDescription
		{
			get
			{
				if (!JsFields.ContainsKey("description"))
					return "";
				return JsFields["description"].ToString();
			}
		}
		public string LinkTarget
		{
			get
			{
				if (!JsFields.ContainsKey("target"))
					return "";
				return JsFields["target"].ToString();
			}
		}
		public string StyleClass
		{
			get
			{
				if (!JsFields.ContainsKey("styleClass"))
					return "";
				return JsFields["styleClass"].ToString();
			}
		}
		public string AltText
		{
			get
			{
				if (!JsFields.ContainsKey("alt"))
					return "";
				return JsFields["alt"].ToString();
			}
		}
		public string QueryString
		{
			get
			{
				if (!JsFields.ContainsKey("query"))
					return "";
				return JsFields["query"].ToString();
			}
		}

		public GeneralLink(string tokenData)
		{
			foreach (string keyval in tokenData.Split(new [] {"|||"}, StringSplitOptions.RemoveEmptyEntries))
			{
				var vals = keyval.Split(new[] {"|=|"}, StringSplitOptions.RemoveEmptyEntries);
				if (vals.Length != 2) continue;
				JsFields.Add(vals[0], vals[1]);
			}
		}

		internal dynamic ConvertToJs()
		{
			return _jsFields;
		}
	}
}
