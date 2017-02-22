using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenManager.Data.Interfaces;

namespace TokenManager.Data.TokenDataTypes
{
	public class StringTokenData : ITokenData
	{
		/// <summary>
		/// Creates a field that gathers a sitecore item's ID from the user in the form of a Sitecore content tree
		/// </summary>
		/// <param name="label">The label displayed to the authors describing what the field is for</param>
		/// <param name="name">The key that's used when placing/retrieving information into the token definition</param>
		/// <param name="placeholder">The text that appears in light gray that is over the top of the input field guiding users suggested input</param>
		/// <param name="required">If true, the user will not be able to leave this field blank</param>

		public StringTokenData(string label, string name, string placeholder, bool required)
		{
			Name = name;
			Label = label;
			Data = new ExpandoObject();
			Data.Placeholder = placeholder;
			Required = required;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }

		public string AngularMarkup
		{
			get { return @"
    <div class=""field-row {{field.class}}"">
        <span class=""field-label"">{{field.Label}} </span>
        <div class=""field-data"">
            <input ng-model=""token.data[field.Name]"" size=""50"" placeholder=""{{field.Data.Placeholder}}"" />
        </div>
    </div>
";
			}
		}

		public dynamic Data { get; set; }
		public object GetValue(string value)
		{
			return value;
		}
	}
}
