using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Version history table view source.
    /// </summary>
    public class VersionHistoryTableViewSource : UITableViewSource
    {
        private const string VersionHistoryCellIdentifier = "1";

        private IList<HistoryItem> items;

        private RowSelectedDelegate rowSelectedCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.IOS.VersionHistoryTableViewSource"/> class.
        /// </summary>
        /// <param name="items">Items.</param>
        /// <param name="rowSelectedCallback">Row selected callback.</param>
        public VersionHistoryTableViewSource(
            IList<HistoryItem> items,
            RowSelectedDelegate rowSelectedCallback)
            : base()
        {
            this.items = items;
            this.rowSelectedCallback = rowSelectedCallback;
        }

        public delegate void RowSelectedDelegate(HistoryItem item);

        /// <summary>
        /// Returns the number of rows in the specified section
        /// </summary>
        /// <returns>The in section.</returns>
        /// <param name="tableview">Tableview.</param>
        /// <param name="section">Section.</param>
        public override int RowsInSection(UITableView tableview, int section)
        {
            return this.items.Count;
        }

        /// <summary>
        /// Gets the height for a specified row
        /// </summary>
        /// <returns>The height for row.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override float GetHeightForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            return VersionHistoryTableViewCell.RowHeight;
        }

        /// <summary>
        /// Gets the height for the header of the specified section
        /// </summary>
        /// <returns>The height for header.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="section">Section.</param>
        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return VersionHistoryTableViewHeader.RowHeight;
        }

        /// <summary>
        /// Returns a view object to display at the start of the given section.
        /// </summary>
        /// <paramref name="section"></paramref>
        /// <returns>The view for header.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="section">Section.</param>
        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            return new VersionHistoryTableViewHeader(tableView.RectForHeaderInSection(section));
        }

        /// <summary>
        /// Returns the cell at the specified index
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
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

        /// <summary>
        /// Method called when a row is selected
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (this.rowSelectedCallback != null)
            {
                this.rowSelectedCallback(this.items[indexPath.Row]);
            }
        }
    }
}