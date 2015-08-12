using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace TokenManager
{
    public static class ItemExtensions
    {
        public static bool IsTokenMangerItem(this Item item)
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
    }
}
