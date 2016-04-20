using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data.TokenDataTypes
{
	public class IntegerTokenData : ITokenData
	{
		public IntegerTokenData(string label, string name, bool required)
		{
			Name = name;
			Label = label;
			Required = required;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }
		public string AngularMarkup => @"
	<div ng-if=""field.Type==2"" class=""field-row {{field.class}}"">
		<span class=""field-label"">{{field.Label}} </span>
		<div class=""field-data"">
			<input type=""number"" ng-model=""token.data[field.Name]"" placeholder=""{{field.Placeholder}}""/>
		</div>
	</div>";
		public dynamic Data { get; }
		public object GetValue(string value)
		{
			return value;
		}
	}
}
