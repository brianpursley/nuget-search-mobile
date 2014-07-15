using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Search result table view cell.
    /// </summary>
    public class SearchResultTableViewCell : UITableViewCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.IOS.SearchResultTableViewCell"/> class.
        /// </summary>
        /// <param name="reuseIdentifier">Reuse identifier.</param>
        public SearchResultTableViewCell(string reuseIdentifier)
            : base(UITableViewCellStyle.Subtitle, reuseIdentifier)
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        /// <summary>
        /// Defines the layout of the controls within the cell
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            ImageView.Frame = new RectangleF(8, 1, 40, 40);
            TextLabel.Frame = new RectangleF(56, TextLabel.Frame.Top, this.Frame.Width - 64, TextLabel.Frame.Height);
            DetailTextLabel.Frame = new RectangleF(56, DetailTextLabel.Frame.Top, this.Frame.Width - 64, DetailTextLabel.Frame.Height);
        }
    }
}