using Sitecore.Data.Items;
using Sitecore.Rules;

namespace TokenManager
{
    public static class ItemExtensions
    {
		/// <summary>
		/// returns true if the item belongs to the token manager
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
        public static bool IsTokenManagerItem(this Item item)
        {
	        if (item == null)
		        return false;
            var parent = item.Parent;
            while (parent != null)
            {
                if (parent.ID.ToString() == Constants.TokenManagerGuid)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }

	    /// <summary>
	    /// runs the set of rules and checks for any matches, if it finds a match it will run the rule's associated action
	    /// </summary>
	    /// <param name="root">Item which holds the field</param>
	    /// <param name="field">the rule field name</param>
	    /// <param name="ruleContext"></param>
	    /// <returns></returns>
	    public static void RunRules<T>(this Item root, string field, T ruleContext)
            where T : RuleContext
        {
            foreach (Rule<T> rule in RuleFactory.GetRules<T>(new[] { root }, field).Rules)
            {
                if (rule.Condition != null)
                {
                    var stack = new RuleStack();
                    rule.Condition.Evaluate(ruleContext, stack);

                    if (ruleContext.IsAborted)
                    {
                        continue;
                    }
                    if ((stack.Count != 0) && ((bool)stack.Pop()))
                    {
                        rule.Execute(ruleContext);
                    }
                }
                else
                    rule.Execute(ruleContext);
            }
        }
    }

}
