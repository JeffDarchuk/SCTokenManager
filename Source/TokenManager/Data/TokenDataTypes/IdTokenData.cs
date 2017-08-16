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
		/// <summary>
		/// Creates a field that gathers a sitecore item's ID from the user in the form of a Sitecore content tree
		/// </summary>
		/// <param name="label">The label displayed to the authors describing what the field is for</param>
		/// <param name="name">The key that's used when placing/retrieving information into the token definition</param>
		/// <param name="required">If true, the user will not be able to leave this field blank</param>
		/// <param name="root">The string GUID that corresponds to the root of the content tree used for item selection</param>
		/// <param name="defaultValue">The starting value of the token data</param>
		public IdTokenData(string label, string name, bool required, string root = "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}", string defaultValue = "")
		{
			Name = name;
			Label = label;
			Required = required;
			Data = new ExpandoObject();
			Data.Root = root;
			DefaultValue = defaultValue;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }
		public string DefaultValue { get; set; }
		public string AngularMarkup => $@"
	<div class=""field-row {{{{ field.class}}}}"">
		<span class=""field-label"">{{{{field.Label}}}} </span>
		<div ng-init=""token.data[field.Name]= token.data[field.Name] ? token.data[field.Name] : '{DefaultValue}';"" class=""field-data"">
			<contenttree parent = ""'{Data.Root}'"" selected=""token.data[field.Name]"" events=""token.events"" ng-click=""token.clicked(field.Name)"" field=""'{Name}'""></contenttree>
		</div>
	</div>";

		public dynamic Data { get; }
		public object GetValue(string value)
		{
			return value;
		}
	}
}
