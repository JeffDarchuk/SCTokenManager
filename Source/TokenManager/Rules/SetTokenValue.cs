using Sitecore;
using Sitecore.Rules.Actions;

namespace TokenManager.Rules
{
	[UsedImplicitly]
	public class SetTokenValue<T> : RuleAction<T>
		where T : TokenRuleContext
	{
		public string Value { get; set; }

		public override void Apply(T ruleContext)
		{
			ruleContext.Value = Value;
		}
	}
}
