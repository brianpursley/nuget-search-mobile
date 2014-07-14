// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace NuGetSearch.IOS
{
	[Register ("PackageDetailScreen")]
	partial class PackageDetailScreen
	{
		[Outlet]
		MonoTouch.UIKit.UILabel authorsCaptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel authorsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView dependenciesTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint dependenciesTableViewHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel descriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint descriptionTopAlignmentConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView detailScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView detailView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint detailViewWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView headerView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint headerViewHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView iconImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel prereleaseLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton projectSiteButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel projectSiteCaptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel tagsCaptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel tagsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel titleLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView versionHistoryTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint versionHistoryTableViewHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel versionLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (authorsCaptionLabel != null) {
				authorsCaptionLabel.Dispose ();
				authorsCaptionLabel = null;
			}

			if (authorsLabel != null) {
				authorsLabel.Dispose ();
				authorsLabel = null;
			}

			if (dependenciesTableView != null) {
				dependenciesTableView.Dispose ();
				dependenciesTableView = null;
			}

			if (dependenciesTableViewHeightConstraint != null) {
				dependenciesTableViewHeightConstraint.Dispose ();
				dependenciesTableViewHeightConstraint = null;
			}

			if (descriptionLabel != null) {
				descriptionLabel.Dispose ();
				descriptionLabel = null;
			}

			if (descriptionTopAlignmentConstraint != null) {
				descriptionTopAlignmentConstraint.Dispose ();
				descriptionTopAlignmentConstraint = null;
			}

			if (detailScrollView != null) {
				detailScrollView.Dispose ();
				detailScrollView = null;
			}

			if (detailView != null) {
				detailView.Dispose ();
				detailView = null;
			}

			if (detailViewWidthConstraint != null) {
				detailViewWidthConstraint.Dispose ();
				detailViewWidthConstraint = null;
			}

			if (headerView != null) {
				headerView.Dispose ();
				headerView = null;
			}

			if (headerViewHeightConstraint != null) {
				headerViewHeightConstraint.Dispose ();
				headerViewHeightConstraint = null;
			}

			if (iconImageView != null) {
				iconImageView.Dispose ();
				iconImageView = null;
			}

			if (prereleaseLabel != null) {
				prereleaseLabel.Dispose ();
				prereleaseLabel = null;
			}

			if (projectSiteCaptionLabel != null) {
				projectSiteCaptionLabel.Dispose ();
				projectSiteCaptionLabel = null;
			}

			if (projectSiteButton != null) {
				projectSiteButton.Dispose ();
				projectSiteButton = null;
			}

			if (tagsCaptionLabel != null) {
				tagsCaptionLabel.Dispose ();
				tagsCaptionLabel = null;
			}

			if (tagsLabel != null) {
				tagsLabel.Dispose ();
				tagsLabel = null;
			}

			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}

			if (versionHistoryTableView != null) {
				versionHistoryTableView.Dispose ();
				versionHistoryTableView = null;
			}

			if (versionHistoryTableViewHeightConstraint != null) {
				versionHistoryTableViewHeightConstraint.Dispose ();
				versionHistoryTableViewHeightConstraint = null;
			}

			if (versionLabel != null) {
				versionLabel.Dispose ();
				versionLabel = null;
			}
		}
	}
}
