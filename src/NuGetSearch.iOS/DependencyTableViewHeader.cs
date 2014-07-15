using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Dependency table view header.
    /// </summary>
    public class DependencyTableViewHeader : UIView
    {
        public const float RowHeight = 17f;

        private static readonly UIFont Font = UIFont.FromName("HelveticaNeue-Bold", 14f);

        private UILabel dependenciesLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.IOS.DependencyTableViewHeader"/> class.
        /// </summary>
        /// <param name="frame">Frame.</param>
        public DependencyTableViewHeader(RectangleF frame)
            : base(frame)
        {
            this.Add(this.dependenciesLabel = new UILabel()
            {
                Font = Font,
                Text = Resources.GetString(Resource.String.dependencies),
                TextColor = UIColor.Black
            });
        }

        /// <summary>
        /// Defines the layout of the controls within the header
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            const float pad = 8;
            float w = this.Bounds.Width - (pad * 2);
            float x = pad;
            const float y = 0;
            this.dependenciesLabel.Frame = new RectangleF(x, y, w, RowHeight);
        }

        /// <summary>
        /// Draws the header
        /// </summary>
        /// <param name="rect">Rect.</param>
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