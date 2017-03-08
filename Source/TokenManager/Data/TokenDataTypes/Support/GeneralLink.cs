using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Web;
using Sitecore.Links;

namespace TokenManager.Data.TokenDataTypes.Support
{
	/// <summary>
	/// Parses a <see cref="GeneralLinkTokenData"/> field's value out of the token's NameValueCollection of data.
	/// 
	/// For example:
	/// 
	/// </summary>
	public class GeneralLink
	{
		private readonly dynamic _jsFields = new ExpandoObject();

		public IDictionary<string, object> JsFields => _jsFields as IDictionary<string, object>;

		public virtual bool Internal
		{
			get
			{
				if (!JsFields.ContainsKey("type"))
					throw new ArgumentException("Malformed token: general link data doesn't contain the field type");

				return JsFields["type"].ToString() == "internal";
			}
		}

		public virtual string ExternalUrl
		{
			get
			{
				object result;
				if (JsFields.TryGetValue("url", out result)) return result.ToString();

				return string.Empty;
			}
		}

		public virtual string InternalLinkId
		{
			get
			{
				object result;
				if (JsFields.TryGetValue("id", out result)) return result.ToString();

				return string.Empty;
			}
		}
		public virtual string LinkDescription
		{
			get
			{
				object result;
				if (JsFields.TryGetValue("description", out result)) return result.ToString();

				return string.Empty;
			}
		}
		public virtual string LinkTarget
		{
			get
			{
				object result;
				if (JsFields.TryGetValue("target", out result)) return result.ToString();

				return string.Empty;
			}
		}
		public virtual string StyleClass
		{
			get
			{
				object result;
				if (JsFields.TryGetValue("styleClass", out result)) return result.ToString();

				return string.Empty;
			}
		}
		public virtual string AltText
		{
			get
			{
				object result;
				if (JsFields.TryGetValue("alt", out result)) return result.ToString();

				return string.Empty;
			}
		}
		public virtual string QueryString
		{
			get
			{
				object result;
				if (JsFields.TryGetValue("query", out result)) return result.ToString();

				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the Href of the current link (normalizing for internal vs external and adding any existing query string specified)
		/// </summary>
		public virtual string Href
		{
			get
			{
				if (Internal)
				{
					var targetItem = (Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database)?.GetItem(InternalLinkId);

					if (targetItem == null) return string.Empty;

					return ProcessUrlQuery(LinkManager.GetItemUrl(targetItem));
				}

				return ProcessUrlQuery(ExternalUrl);
			}
		}

		public GeneralLink(string tokenData)
		{
			if (tokenData == null) return;
			foreach (string keyval in tokenData.Split(new [] {"|||"}, StringSplitOptions.RemoveEmptyEntries))
			{
				var vals = keyval.Split(new[] {"|=|"}, StringSplitOptions.RemoveEmptyEntries);
				if (vals.Length != 2) continue;
				JsFields.Add(HttpUtility.HtmlDecode(vals[0]), HttpUtility.HtmlDecode(vals[1]));
			}
		}

		/// <summary>
		/// Renders an opening anchor tag for this token field (you must emit the end tag and body yourself)
		/// </summary>
		/// <param name="overrideClass">If specified will override any class specified on the field</param>
		/// <param name="overrideTarget">If specified will override any target specified on the field</param>
		/// <returns>HTML string of the link opening tag</returns>
		public virtual string RenderOpeningAnchorTag(string overrideClass = null, string overrideTarget = null)
		{
			var target = overrideTarget ?? LinkTarget;
			var cssClass = overrideClass ?? StyleClass;

			var builder = new StringBuilder();

			builder.Append($"<a href=\"{HttpUtility.HtmlEncode(Href)}\"");

			if (!string.IsNullOrWhiteSpace(AltText)) builder.Append($" title=\"{HttpUtility.HtmlEncode(AltText)}\"");
			if (!string.IsNullOrWhiteSpace(target)) builder.Append($" target=\"{HttpUtility.HtmlEncode(target)}\"");
			if (!string.IsNullOrWhiteSpace(cssClass)) builder.Append($" class=\"{HttpUtility.HtmlEncode(cssClass)}\"");

			builder.Append(">");

			return builder.ToString();
		}

		/// <summary>
		/// Renders a complete anchor tag for this token field (using the Description as the body)
		/// </summary>
		/// <param name="overrideClass">If specified will override any class specified on the field</param>
		/// <param name="overrideTarget">If specified will override any target specified on the field</param>
		/// <param name="overrideDescription">If specified will override any description specified on the field (overrides link text)</param>
		/// <param name="defaultDescription">If specified will set a default value for the link body if the Description is not set.</param>
		/// <returns>HTML string of the link tag</returns>
		public virtual string RenderAnchorTag(string overrideClass = null, string overrideTarget = null, string overrideDescription = null, string defaultDescription = null)
		{
			var description = overrideDescription ?? (string.IsNullOrWhiteSpace(LinkDescription) ? defaultDescription : LinkDescription);

			return string.Concat(RenderOpeningAnchorTag(overrideClass, overrideTarget), HttpUtility.HtmlEncode(description ?? string.Empty), "</a>");
		}

		protected virtual string ProcessUrlQuery(string baseUrl)
		{
			if (string.IsNullOrWhiteSpace(baseUrl)) return string.Empty;

			if (string.IsNullOrWhiteSpace(QueryString)) return baseUrl;

			return $"{baseUrl}?{QueryString.TrimStart('?')}";
		}

		internal dynamic ConvertToJs()
		{
			return _jsFields;
		}
	}
}
