using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;

namespace TokenManager
{

	/// <summary>
	/// from: http://laubplusco.net/sitecore-extensions-does-a-sitecore-item-derive-from-a-template/
	/// </summary>
	public static class TemplateExtensions
	{
		/// <summary>
		/// returns true if the template object is derived from the template id given
		/// </summary>
		/// <param name="template"></param>
		/// <param name="templateId"></param>
		/// <returns></returns>
		public static bool IsDerived(this Template template, ID templateId)
		{
		    if (template == null) return false;
			return template.ID == templateId || template.GetBaseTemplates().Any(baseTemplate => IsDerived(baseTemplate, templateId));
		}

		/// <summary>
		/// returns true if the template object is derived from the template id given
		/// </summary>
		/// <param name="template"></param>
		/// <param name="templateId"></param>
		/// <returns></returns>
		public static bool IsDerived( this TemplateItem template,  ID templateId)
	    {
            if (template == null) return false;
            return template.ID == templateId || template.BaseTemplates.Any(baseTemplate => IsDerived(baseTemplate, templateId));
	    }
	}
}
