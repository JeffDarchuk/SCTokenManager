using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data.TokenDataTypes
{
	public class BooleanTokenData : ITokenData
	{
		public BooleanTokenData(string label, string name, bool required)
		{
			Name = name;
			Label = label;
			Required = required;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }

		public string AngularMarkup
		{
			get { return @"
<div ng-if=""field.Type==3"" class=""field-row {{field.class}}"">
		<span class=""field-label"">{{field.Label}} </span>
		<div class=""field-data"">
			<input type=""checkbox"" ng-model=""token.data[field.Name]""/>
		</div>
	</div>
"; }
		}

		public dynamic Data { get; }
		public object GetValue(string value)
		{
			return value == "True";
		}
	}
}
