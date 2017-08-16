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
		/// <summary>
		/// Creates a field that gathers an integer from the user in the form of an number input field
		/// </summary>
		/// <param name="label">The label displayed to the authors describing what the field is for</param>
		/// <param name="name">The key that's used when placing/retrieving information into the token definition</param>
		/// <param name="required">If true, the user will not be able to leave this field blank</param>
		/// <param name="defaultValue">The starting value of the token data</param>
		public IntegerTokenData(string label, string name, bool required, int defaultValue = 0)
		{
			Name = name;
			Label = label;
			Required = required;
			DefaultValue = defaultValue;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }
		public int DefaultValue { get; set; }
		public string AngularMarkup => $@"
	<div class=""field-row {{{{field.class}}}}"">
		<span class=""field-label"">{{{{field.Label}}}} </span>
		<div ng-init=""token.data[field.Name]= token.data[field.Name] ? token.data[field.Name] : {DefaultValue};""  class=""field-data"">
			<input type=""number"" ng-model=""token.data[field.Name]"" placeholder=""{{{{field.Placeholder}}}}""/>
		</div>
	</div>";
		public dynamic Data { get; }
		public object GetValue(string value)
		{
			int ret;
			if (int.TryParse(value, out ret))
				return ret;
			return "";
		}
	}
}
