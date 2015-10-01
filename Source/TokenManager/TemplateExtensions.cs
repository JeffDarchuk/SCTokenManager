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
		public static bool IsDerived(this Template template, ID templateId)
		{
		    if (template == null) return false;
			return template.ID == templateId || template.GetBaseTemplates().Any(baseTemplate => IsDerived(baseTemplate, templateId));
		}

	    public static bool IsDerived( this TemplateItem template,  ID templateId)
	    {
            if (template == null) return false;
            return template.ID == templateId || template.BaseTemplates.Any(baseTemplate => IsDerived(baseTemplate, templateId));
	    }
	}
}
