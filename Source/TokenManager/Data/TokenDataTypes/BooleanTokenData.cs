using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.TokenDataTypes
{
	public class BooleanTokenData : ITokenData
	{
		/// <summary>
		/// Creates a field that gathers a boolean from the user in the form of a checkbox
		/// </summary>
		/// <param name="label">The label displayed to the authors describing what the field is for</param>
		/// <param name="name">The key that's used when placing/retrieving information into the token definition</param>
		/// <param name="defaultValue">The starting value of the token data</param>
		public BooleanTokenData(string label, string name, bool defaultValue = false)
		{
			Name = name;
			Label = label;
			Required = false;
			DefaultValue = defaultValue;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }
		public bool DefaultValue { get; set; }
		public string AngularMarkup => $@"
	<div class=""field-row {{{{field.class}}}}"">
		<span class=""field-label"">{{{{field.Label}}}} </span>
		<div ng-init=""token.data[field.Name]= token.data[field.Name] ? token.data[field.Name] : {(DefaultValue ? "true" : "false")};"" class=""field-data"">
			<input type=""checkbox"" ng-model=""token.data[field.Name]""/>
		</div>
	</div>
";

		public dynamic Data { get; }
		public object GetValue(string value)
		{
			return value == "True";
		}
	}
}
