using System;
using System.Drawing;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace NuGetSearch.IOS
{
    public class VersionHistoryTableViewHeader : UIView
    {
        public const float RowHeight = 17f;

        private static readonly UIFont font = UIFont.FromName("HelveticaNeue-Bold", 14f);

        private UILabel versionLabel;
        private UILabel downloadCountLabel;
        private UILabel lastUpdatedLabel;

        public VersionHistoryTableViewHeader(RectangleF frame)
            : base(frame)
        {
            this.Add(this.versionLabel = new UILabel()
            {
                Font = font,
                Text = Resources.GetString(Resource.String.version_history),
                TextColor = UIColor.Black
            });

            this.Add(this.downloadCountLabel = new UILabel()
            {
                Font = font,
                Text = Resources.GetString(Resource.String.downloads),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Right
            });

            this.Add(this.lastUpdatedLabel = new UILabel()
            {
                Font = font,
                Text = Resources.GetString(Resource.String.last_updated),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Right
            });
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            const float pad = 8;
            float w = this.Bounds.Width - pad * 2;
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

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);
            using (var g = UIGraphics.GetCurrentContext())
            {
                UIColor.FromRGB(240, 240, 240).SetFill();
                g.FillRect(rect);
                g.MoveTo(rect.Left, rect.Bottom);
                g.SetLineWidth(1);
                UIColor.FromRGB(192, 192, 192).SetStroke();
                g.AddLineToPoint(rect.Right, rect.Bottom);
                g.StrokePath();
            }
        }
    }
}

