using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace NuGetSearch.IOS
{
    public class SearchResultTableViewCell : UITableViewCell
    {
        public SearchResultTableViewCell(string reuseIdentifier)
            : base(UITableViewCellStyle.Subtitle, reuseIdentifier)
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            base.ImageView.Frame = new RectangleF(8, 1, 40, 40);
            base.TextLabel.Frame = new RectangleF(56, base.TextLabel.Frame.Top, this.Frame.Width - 64, base.TextLabel.Frame.Height);
            base.DetailTextLabel.Frame = new RectangleF(56, base.DetailTextLabel.Frame.Top, this.Frame.Width - 64, base.DetailTextLabel.Frame.Height);
        }
    }
}

