using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using NuGetSearch.Common;
using MonoTouch.Foundation;

namespace NuGetSearch.IOS
{
    public class VersionHistoryTableViewSource : UITableViewSource
    {
        private const string VersionHistoryCellIdentifier = "1";

        private IList<HistoryItem> items;

        private RowSelectedDelegate rowSelectedCallback;

        public delegate void RowSelectedDelegate(HistoryItem item);

        public VersionHistoryTableViewSource(
            IList<HistoryItem> items,
            RowSelectedDelegate rowSelectedCallback)
            : base()
        {
            this.items = items;
            this.rowSelectedCallback = rowSelectedCallback;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return this.items.Count;
        }

        public override float GetHeightForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            return VersionHistoryTableViewCell.RowHeight;
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return VersionHistoryTableViewHeader.RowHeight;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            return new VersionHistoryTableViewHeader(tableView.RectForHeaderInSection(section));
        }

        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            var position = indexPath.Row;

            // Get the item at this position
            var item = this.items[position];  

            // Re-use the cell that was passed in the convertView argument, or create a new cell if it was null
            var cell = tableView.DequeueReusableCell(VersionHistoryCellIdentifier) as VersionHistoryTableViewCell
                ?? new VersionHistoryTableViewCell(VersionHistoryCellIdentifier);

            cell.UpdateCell(item);

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (rowSelectedCallback != null)
            {
                rowSelectedCallback(this.items[indexPath.Row]);
            }
        }

    }
}