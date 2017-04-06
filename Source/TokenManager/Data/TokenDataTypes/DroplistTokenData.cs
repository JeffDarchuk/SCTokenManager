using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.TokenDataTypes
{
	public class DroplistTokenData : ITokenData
	{
		private string _options;
		/// <summary>
		/// Creates a field that gathers a string value from a list of options in the form of a dropdown
		/// </summary>
		/// <param name="label">The label displayed to the authors describing what the field is for</param>
		/// <param name="name">The key that's used when placing/retrieving information into the token definition</param>
		/// <param name="required">If true, the user will not be able to leave this field blank</param>
		/// <param name="options">Inumerable of key value pairs to use in creation of the dropdown list, the keys are used as the labels, and the values are the option values</param>
		public DroplistTokenData(string label, string name, bool required, IEnumerable<KeyValuePair<string, string>> options)
		{
			Name = name;
			Label = label;
			Required = required;
			StringBuilder sb = new StringBuilder();
			foreach (var option in options)
				sb.Append($"<option name=\"{option.Value}\" value=\"{option.Value}\">{option.Key}</option>");
			_options = sb.ToString();
		}
		/// <summary>
		/// Creates a field that gathers a string value from a list of options in the form of a dropdown
		/// </summary>
		/// <param name="label">The label displayed to the authors describing what the field is for</param>
		/// <param name="name">The key that's used when placing/retrieving information into the token definition</param>
		/// <param name="required">If true, the user will not be able to leave this field blank</param>
		/// <param name="options">Inumerable of key value pairs to use in creation of the dropdown list, the values will be used as the label and value for the options</param>
		public DroplistTokenData(string label, string name, bool required, IEnumerable<string> options) :this(label, name, required, options.Select(x => new KeyValuePair<string, string>(x, x)))
		{
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }
		public string AngularMarkup => $@"
	<div class=""field-row {{{{field.class}}}}"">
		<span class=""field-label"">{{{{field.Label}}}} </span>
		<div class=""field-data"">
			<select ng-model=""token.data[field.Name]"">
				{_options}
			</select>
		</div>
	</div>";
		public dynamic Data { get; }
		public object GetValue(string value)
		{
			return value;
		}
	}
}
