using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    public class VersionHistoryTableViewCell : UITableViewCell  
    {
        public const float RowHeight = 23f;

        private static readonly UIFont font = UIFont.FromName("HelveticaNeue", 14f);

        private UILabel versionLabel;
        private UILabel downloadCountLabel;
        private UILabel lastUpdatedLabel;

        public VersionHistoryTableViewCell(string reuseIdentifier)
            : this(new NSString(reuseIdentifier))
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public VersionHistoryTableViewCell(NSString reuseIdentifier) 
            : base (UITableViewCellStyle.Default, reuseIdentifier)
        {
            this.ContentView.BackgroundColor = UIColor.White;

            this.ContentView.Add(this.versionLabel = new UILabel()
            {
                Font = font,
            });

            this.ContentView.Add(this.downloadCountLabel = new UILabel()
            {
                Font = font,
                TextColor = UIColor.Gray,
                TextAlignment = UITextAlignment.Right
            });

            this.ContentView.Add(this.lastUpdatedLabel = new UILabel()
            {
                Font = font,
                TextColor = UIColor.Gray,
                TextAlignment = UITextAlignment.Right
            });
        }

        public void UpdateCell(HistoryItem historyItem)
        {
            this.versionLabel.Text = historyItem.Version;
            this.downloadCountLabel.Text = historyItem.VersionDownloadCount.ToString("n0");
            this.lastUpdatedLabel.Text = historyItem.Published.ToShortDateString();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            const float pad = 8;
            float w = this.ContentView.Bounds.Width - pad * 2;
            float w1 = (w / 3) - pad;
            float w2 = (w / 3) - pad;
            float w3 = (w / 3);
            const float y = 0;
            float x1 = pad;
            float x2 = x1 + w1 + pad;
            float x3 = x2 + w2 + pad;
            this.versionLabel.Frame = new RectangleF(x1, y, w1, RowHeight);
            this.downloadCountLabel.Frame = new RectangleF(x2, y, w2, RowHeight);
            this.lastUpdatedLabel.Frame = new RectangleF(x3, y, w3, RowHeight);
        }
    }
}

