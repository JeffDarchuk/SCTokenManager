using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes;
using TokenManager.Data.TokenDataTypes.Support;
using TokenManager.Data.TokenExtensions;
using TokenManager.Data.Tokens;

namespace TokenManager
{
	public class DemoViewToken : ViewAutoToken<DemoAutoToken2Model>
	{
		//Make sure you have a parameterless constructor.
		public DemoViewToken() : base(collectionName: "Demo View Collection", tokenIcon: "people/16x16/cubes_blue.png", tokenName: "Demo View Token")
		{
		}
		/// <summary>
		/// Given the populated model, return the full cshtml path from the webroot.
		/// </summary>
		/// <param name="extraData">The populated model</param>
		/// <returns>full path to the cshtml file.</returns>
		public override string GetViewPath(DemoAutoToken2Model model)
		{
			return "/views/DemoViewToken.cshtml";
		}

		/// <summary>
		/// Assign an RTE button for this token.
		/// </summary>
		/// <returns>TokenButton object used to render the button for this token.</returns>
		public override TokenButton TokenButton()
		{
			return new TokenButton(name: "Insert a demo snippet", icon: "people/16x16/cubes_blue.png", sortOrder: 250);
		}

		/// <summary>
		/// Modifies the default way the token is displayed in the RTE
		/// </summary>
		/// <param name="model">The tokens model</param>
		/// <returns>Markup for the token for the RTE</returns>
		public override string TokenIdentifierText(DemoAutoToken2Model model)
		{
			return $"<span style='color:red;font-size:large'>hey</span><span>now</span>{model.MySecretValue}";
		}

		public override bool IsCurrentContextValid(Item item = null)
		{
			// Only available on items that's name start with bob;
			return item?.Name.StartsWith("bob") ?? false;
		}

		public override IEnumerable<ID> ValidParents()
		{
			// only available if in a subtree rooted at these items
			yield return new ID("11111111-1111-1111-1111-111111111111");
		}

		public override IEnumerable<ID> ValidTemplates()
		{
			// only available on items with this template
			yield return new ID("11111111-1111-1111-1111-111111111111");
		}

		public override string Render(DemoAutoToken2Model model)
		{
			model.UserName = Sitecore.Context.User.Name;
			return base.Render(model);
		}
	}
}
