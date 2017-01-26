using System.Dynamic;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes.Support;

namespace TokenManager.Data.TokenDataTypes
{
	public class GeneralLinkTokenData : ITokenData
	{
		/// <summary>
		/// Adds an analog of a general link field into a token's data fields. The link is tracked by the links database.
		/// </summary>
		/// <param name="label">The label that is shown for this field when editing the token</param>
		/// <param name="name">The key in the token data collection this will be stored in (use this when getting the value out of the NameValueCollection; use the <see cref="GeneralLink" /> class to parse the retrieved string)</param>
		/// <param name="required">Whether this token field is required or not</param>
		/// <param name="root">For internal links, this sets the item ID of the root item in the content tree to choose an item from</param>
		public GeneralLinkTokenData(string label, string name, bool required, string root = "{0DE95AE4-41AB-4D01-9EB0-67441B7C2450}")
		{
			Name = name;
			Label = label;
			Required = required;
			Data = new ExpandoObject();
			Data.root = root;
		}
		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }

		public string AngularMarkup
		{
			get { return $@"<div class=""field-row {{{{ field.class}}}}"">
				<span class=""field-label"">{{{{field.Label}}}} </span>
				<div class=""field-data"">
					<label><input type=""radio"" ng-model=""token.data[field.Name].grouped.type"" value=""external""><span style='position:relative;top:-2px'>External Link</span></label><br/>
					<label style=""margin-bottom:4px""><input type=""radio"" ng-model=""token.data[field.Name].grouped.type"" value=""internal""><span style='position:relative;top:-2px'>Internal Link</span></label><br/>
					<div ng-if=""token.data[field.Name].grouped.type == 'internal'"">
						<label>Sitecore Internal Link Item</label>
						<contenttree parent = ""'{Data.root}'"" selected=""token.data[field.Name].grouped.id"" events=""token.events"" ng-click=""token.groupedClicked(field.Name)""></contenttree>
					</div>
					<div ng-if=""token.data[field.Name].grouped.type == 'external'"">
						<label>External Url</label>
						<input type=""text"" placeholder=""Enter external url"" ng-model=""token.data[field.Name].grouped.url"" />
					</div>
					<div ng-if=""token.data[field.Name].grouped.type"">
						<label>Link Description</label>
						<input type=""text"" ng-model=""token.data[field.Name].grouped.description"" />
						<label>Link Target</label>
						<select ng-model=""token.data[field.Name].grouped.target"">
							<option value=""_blank"">Load in a new window</option>
							<option value=""_self"">Load in the same window</option>
						</select>
						<label>Style Class</label>
						<input type=""text"" ng-model=""token.data[field.Name].grouped.styleClass"" />
						<label>Alt Text</label>
						<input type=""text"" ng-model=""token.data[field.Name].grouped.alt"" />
						<label>Query String</label>
						<input type=""text"" ng-model=""token.data[field.Name].grouped.query"" />
					</div>
			</div>
			</div>"; }
		}

		public dynamic Data { get; }

		public object GetValue(string value)
		{
			dynamic ret = new ExpandoObject();
			ret.grouped = new GeneralLink(value).ConvertToJs();
			return ret;
		}
	}
}
