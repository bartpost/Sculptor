using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.TreeListView;
using System;

namespace RadTreeListView_SL
{
	public class CommodityRowStyle : StyleSelector
	{
		public override Style SelectStyle(object item, DependencyObject container)
		{
			TreeListViewRow row = container as TreeListViewRow;
			if (item is Club)
			{
				Club parent = item as Club;
				if (parent.Players.Count > 1)
				{
					Dispatcher.BeginInvoke(new Action(() => row.IsExpanded = true));					
					return ExpandedStyle;
				}
				else
				{
					Dispatcher.BeginInvoke(new Action(() => row.IsExpanded = false));
					return CollapseStyle;
				}
			}
			return null;
		}

		public Style CollapseStyle { get; set; }
		public Style ExpandedStyle { get; set; }
	}
}
