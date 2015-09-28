using Sitecore.Data.Items;
using Sitecore.Rules;

namespace TokenManager
{
    public static class ItemExtensions
    {
        public static bool IsTokenManagerItem(this Item item)
        {
            var parent = item.Parent;
            while (parent != null)
            {
                if (parent.ID.ToString() == Constants._tokenManagerGuid)
                    return true;
                parent = parent.Parent;
            }
            return false;
        }
        /// <summary>
        /// runs the set of rules and checks for any matches, if it finds a match it will run the rule's associated action
        /// </summary>
        /// <param name="Root">Item which holds the field</param>
        /// <param name="Field">the rule field name</param>
        /// <returns></returns>
        public static void RunRules<T>(this Item Root, string Field, T ruleContext)
            where T : RuleContext
        {
            foreach (Rule<T> rule in RuleFactory.GetRules<T>(new[] { Root }, Field).Rules)
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
