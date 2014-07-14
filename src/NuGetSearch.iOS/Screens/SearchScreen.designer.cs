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
	[Register ("SearchScreen")]
	partial class SearchScreen
	{
		[Outlet]
		MonoTouch.UIKit.UISwitch includePrereleaseSwitch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel noNetworkLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar searchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView searchResultsTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView statsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel totalPackageDownloadsCaptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel totalPackageDownloadsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel totalPackagesCaptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel totalPackagesLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel uniquePackagesCaptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel uniquePackagesLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (includePrereleaseSwitch != null) {
				includePrereleaseSwitch.Dispose ();
				includePrereleaseSwitch = null;
			}

			if (noNetworkLabel != null) {
				noNetworkLabel.Dispose ();
				noNetworkLabel = null;
			}

			if (searchBar != null) {
				searchBar.Dispose ();
				searchBar = null;
			}

			if (searchResultsTableView != null) {
				searchResultsTableView.Dispose ();
				searchResultsTableView = null;
			}

			if (statsView != null) {
				statsView.Dispose ();
				statsView = null;
			}

			if (totalPackageDownloadsCaptionLabel != null) {
				totalPackageDownloadsCaptionLabel.Dispose ();
				totalPackageDownloadsCaptionLabel = null;
			}

			if (totalPackageDownloadsLabel != null) {
				totalPackageDownloadsLabel.Dispose ();
				totalPackageDownloadsLabel = null;
			}

			if (totalPackagesCaptionLabel != null) {
				totalPackagesCaptionLabel.Dispose ();
				totalPackagesCaptionLabel = null;
			}

			if (totalPackagesLabel != null) {
				totalPackagesLabel.Dispose ();
				totalPackagesLabel = null;
			}

			if (uniquePackagesCaptionLabel != null) {
				uniquePackagesCaptionLabel.Dispose ();
				uniquePackagesCaptionLabel = null;
			}

			if (uniquePackagesLabel != null) {
				uniquePackagesLabel.Dispose ();
				uniquePackagesLabel = null;
			}
		}
	}
}
