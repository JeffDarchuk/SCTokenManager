using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data.TokenDataTypes
{
	public class IdTokenData : ITokenData
	{
		public IdTokenData(string label, string name, bool required, string root = "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}")
		{
			Name = name;
			Label = label;
			Required = required;
			Data = new ExpandoObject();
			Data.Root = root;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }

		public string AngularMarkup
		{
			get { return $@"<div class=""field-row {{{{ field.class}}}}"">
		<span class=""field-label"">{{{{field.Label}}}} </span>
		<div class=""field-data"">
			<contenttree parent = ""'{Data.Root}'"" selected=""token.data[field.Name]"" events=""token.events"" ng-click=""token.clicked(field.Name)""></contenttree>
		</div>
	</div>"; }
		}

		public dynamic Data { get; }
		public object GetValue(string value)
		{
			return value;
		}
	}
}
