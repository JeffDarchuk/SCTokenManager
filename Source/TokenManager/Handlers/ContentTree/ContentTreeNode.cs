using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.StringExtensions;

namespace TokenManager.Handlers.ContentTree
{
	public class ContentTreeNode
	{
		public string Icon = "";
		public string DisplayName;
		public ID Id;
		public bool Open;
		public List<ContentTreeNode> Nodes;

		public ContentTreeNode()
		{
		}

		public ContentTreeNode(Item item, bool open = true)
		{
			Open = open;
			SetIcon(item);
			DisplayName = item.DisplayName;
			Id = item.ID;
			if (Open)
				Nodes = item.Children.Select(c => new ContentTreeNode(c)).ToList();
		}

		public void SetIcon(Item item)
		{
			if (item != null)
			{

				Icon = null;
				Icon = item[FieldIDs.Icon];
				if (Icon.IsNullOrEmpty() && item.Template != null)
				{
					Icon = item.Template.Icon;
				}
			}
		}
	}
}
