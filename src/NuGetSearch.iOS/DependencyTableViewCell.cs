using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Dependency table view cell.
    /// </summary>
    public class DependencyTableViewCell : UITableViewCell  
    {
        public const float RowHeight = 40f;

        private static readonly UIFont Font = UIFont.FromName("HelveticaNeue", 14f);

        private UILabel titleLabel;
        private UILabel versionRangeLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.IOS.DependencyTableViewCell"/> class.
        /// </summary>
        /// <param name="reuseIdentifier">Reuse identifier.</param>
        public DependencyTableViewCell(string reuseIdentifier)
            : this(new NSString(reuseIdentifier))
        {
            this.SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.IOS.DependencyTableViewCell"/> class.
        /// </summary>
        /// <param name="reuseIdentifier">Reuse identifier.</param>
        public DependencyTableViewCell(NSString reuseIdentifier) 
            : base(UITableViewCellStyle.Default, reuseIdentifier)
        {
            this.ContentView.BackgroundColor = UIColor.White;

            this.ContentView.Add(this.titleLabel = new UILabel()
            {
                Font = Font,
            });

            this.ContentView.Add(this.versionRangeLabel = new UILabel()
            {
                Font = Font,
                TextColor = UIColor.Gray,
            });
        }

        /// <summary>
        /// Displays the specified dependency in the cell
        /// </summary>
        /// <param name="dependency">Dependency.</param>
        public void UpdateCell(PackageDependency dependency)
        {
            this.titleLabel.Text = dependency.Title;
            this.versionRangeLabel.Text = string.IsNullOrEmpty(dependency.VersionRange) ? 
                string.Empty : 
                string.Format("({0})", dependency.VersionRange);
        }

        /// <summary>
        /// Defines the layout of the controls within the cell
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            const float pad = 8;
            float w = this.ContentView.Bounds.Width - (pad * 2);
            const float h1 = 17;
            const float h2 = 17;
            float x = pad;
            const float y1 = 3;
            const float y2 = 20;
            this.titleLabel.Frame = new RectangleF(x, y1, w, h1);
            this.versionRangeLabel.Frame = new RectangleF(x, y2, w, h2);
        }
    }
}