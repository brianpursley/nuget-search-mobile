using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Package detail screen
    /// </summary>
    public partial class PackageDetailScreen : UIViewController
    {
        private INuGetGalleryClient nugetGalleryClient;
        private INetworkChecker networkChecker;

        private LoadingOverlay loadingOverlay = null;

        private string packageId;
        private string packageTitle;

        public PackageDetailScreen(string packageId, string packageTitle)
            : base("PackageDetailScreen", null)
        {
            this.packageId = packageId;
            this.packageTitle = packageTitle;

            this.nugetGalleryClient = new NuGetGalleryClient(new NetworkProvider());
            this.networkChecker = new IOSNetworkChecker();  
        }

        private static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.ShowLoadingOverlay(Resource.String.wait_loading);

            // Set default icon image until the real icon can be loaded
            this.iconImageView.Image = Resources.GetImage(Resource.Image.packageDefaultIcon_png);

            this.titleLabel.Hidden = true;
            this.versionLabel.Hidden = true;
            this.prereleaseLabel.Hidden = true;
            this.descriptionLabel.Hidden = true;
            this.projectSiteCaptionLabel.Hidden = true;
            this.projectSiteButton.Hidden = true;
            this.authorsCaptionLabel.Hidden = true;
            this.authorsLabel.Hidden = true;
            this.tagsCaptionLabel.Hidden = true;
            this.tagsLabel.Hidden = true;
            this.dependenciesTableView.Hidden = true;
            this.versionHistoryTableView.Hidden = true;

            this.projectSiteCaptionLabel.Text = Resources.GetString(Resource.String.project_site);
            this.authorsCaptionLabel.Text = Resources.GetString(Resource.String.authors);
            this.tagsCaptionLabel.Text = Resources.GetString(Resource.String.tags);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, true);
            this.DisplayPackageDetailsAsync();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            var y = this.versionLabel.Frame.Bottom + 8;
            if (y > this.headerViewHeightConstraint.Constant)
            {
                this.headerViewHeightConstraint.Constant = y;
            }

            this.versionHistoryTableViewHeightConstraint.Constant = 
                PackageDetailScreen.GetTableViewCellsHeight(this.versionHistoryTableView, 0);

            var w = this.View.Frame.Width;
            this.detailViewWidthConstraint.Constant = w;

            this.titleLabel.PreferredMaxLayoutWidth = w;
            this.versionLabel.PreferredMaxLayoutWidth = w;
            this.prereleaseLabel.PreferredMaxLayoutWidth = w;
            this.descriptionLabel.PreferredMaxLayoutWidth = w;
            this.projectSiteButton.TitleLabel.PreferredMaxLayoutWidth = w;
            this.authorsLabel.PreferredMaxLayoutWidth = w;
            this.tagsLabel.PreferredMaxLayoutWidth = w;
        }
         
        /// <summary>
        /// Displays the package details
        /// </summary>
        /// <returns>Task</returns>
        private Task DisplayPackageDetailsAsync()
        {
            // Check if there is network connectivity, an if there is not, then return
            if (!this.networkChecker.ValidateNetworkConnectivity()) 
            { 
                return Task.Run(() => { }); 
            }   

            var packageUrl = this.packageId;

            return Task.Run(() => 
            {
                // If the package url was not specified, then retrieve the package url of the latest version based on the title
                if (string.IsNullOrEmpty(packageUrl))
                {
                    packageUrl = nugetGalleryClient.GetPackageLatestIdAsync(this.packageTitle).Result;
                }

                // Retrieve package details
                var pd = nugetGalleryClient.GetPackageDetailAsync(packageUrl).Result;

                // Display the package details
                this.InvokeOnMainThread(() => { this.DisplayPackageDetails(pd); });

                // Retrieve the version history
                var history = nugetGalleryClient.GetPackageHistoryAsync(pd.Title).Result;

                // Display the version history
                this.InvokeOnMainThread(() => { this.DisplayVersionHistory(pd.Id, history); });

                this.InvokeOnMainThread(() => { this.HideLoadingOverlay(); });
            });
        }

        /// <summary>
        /// Displays the package details
        /// </summary>
        /// <param name="pd">The package details to display</param>
        private void DisplayPackageDetails(PackageDetail pd)
        {
            // Tell the icon manager to load the icon
            IOSIconManager.Current.Load(pd.IconUrl, x => { this.InvokeOnMainThread(() => { this.DisplayIcon(x); }); });

            // Populate the header fields (title and version)
            this.titleLabel.Text = pd.DisplayTitle;
            this.titleLabel.Hidden = false;
            this.versionLabel.Text = pd.Version;
            this.versionLabel.Hidden = false;

            // Show the header layout
            this.headerView.Hidden = false;

            // Check if this package is prerelease and if so, show the prerelease indicator
            this.prereleaseLabel.Hidden = !pd.IsPrerelease;

            // Adjust the description position to account for the prerelease label being visible or not
            this.descriptionTopAlignmentConstraint.Constant = this.prereleaseLabel.Hidden ? 0 : 25;

            // Populate the description
            this.descriptionLabel.Text = pd.Description;
            this.descriptionLabel.Hidden = false;

            // Display project site link, authors, tags, and dependencies list
            this.DisplayProjectSite(pd);
            this.DisplayAuthors(pd);
            this.DisplayTags(pd);
            this.DisplayDependencies(pd);
        }

        /// <summary>
        /// Displays the project site link from the specified package details
        /// </summary>
        /// <param name="pd"></param>
        private void DisplayProjectSite(PackageDetail pd)
        {
            if (string.IsNullOrEmpty(pd.ProjectUrl))
            {
                this.projectSiteButton.SetTitle(Resources.GetString(Resource.String.none), UIControlState.Normal);
            }
            else
            {
                this.projectSiteButton.SetTitle(pd.ProjectUrl, UIControlState.Normal);
                this.projectSiteButton.TouchUpInside += (sender, e) => 
                {
                    UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(pd.ProjectUrl));
                };
            }
            this.projectSiteCaptionLabel.Hidden = false;
            this.projectSiteButton.Hidden = false;
        }

        /// <summary>
        /// Displays the authors for the specified package details
        /// </summary>
        /// <param name="pd"></param>
        private void DisplayAuthors(PackageDetail pd)
        {
            this.authorsLabel.Text = pd.Authors.Any() ? 
                string.Join(", ", pd.Authors.ToArray()) :
                Resources.GetString(Resource.String.none);
            this.authorsCaptionLabel.Hidden = false;
            this.authorsLabel.Hidden = false;
        }

        /// <summary>
        /// Displays the tags for the specified package details
        /// </summary>
        /// <param name="pd"></param>
        private void DisplayTags(PackageDetail pd)
        {
            this.tagsLabel.Text = pd.Tags.Any() ? 
                string.Join(", ", pd.Tags.ToArray()) :
                Resources.GetString(Resource.String.none);
            this.tagsCaptionLabel.Hidden = false;
            this.tagsLabel.Hidden = false;
        }

        /// <summary>
        /// Displays the specified icon
        /// </summary>
        /// <param name="url">The url of the icon to display</param>
        private void DisplayIcon(string url)
        {
            // Check if the icon manager has finished loading the bitmap or not
            if (IOSIconManager.Current.IsLoaded(url))
            {
                // If the image has been loaded, then get it from the icon manager and set it on the image view
                var icon = IOSIconManager.Current.GetIcon(url);
                if (icon == null)
                {
                    // If the image failed to load, then set the default icon image
                    this.iconImageView.Image = Resources.GetImage(Resource.Image.packageDefaultIcon_png);
                }
                else
                {
                    try
                    {
                        this.iconImageView.Image = icon;
                    }
                    catch
                    {
                        this.iconImageView.Image = Resources.GetImage(Resource.Image.packageDefaultIcon_png);
                    }
                }
            }
            else
            {
                // If the icon manager has not finished loading the image, then set the default icon image for now
                this.iconImageView.Image = Resources.GetImage(Resource.Image.packageDefaultIcon_png);
            }
        }

        /// <summary>
        /// Displays the dependencies from the specified package details
        /// </summary>
        /// <param name="pd"></param>
        private void DisplayDependencies(PackageDetail pd)
        {
            if (pd.Dependencies.Any())
            {
                this.dependenciesTableView.Source = new DependencyTableViewSource(pd.Dependencies, this.DependencySelected);
                this.dependenciesTableView.ReloadData();
                this.dependenciesTableViewHeightConstraint.Constant = 
                    PackageDetailScreen.GetTableViewCellsHeight(this.dependenciesTableView, 0);
                this.dependenciesTableView.Hidden = false;
            }
        }

        /// <summary>
        /// Displays version history from the specified package details
        /// </summary>
        /// <param name="currentId">The id of the version that is currently being displayed</param>
        /// <param name="history">The history items that make up the version history</param>
        private void DisplayVersionHistory(string currentId, IEnumerable<HistoryItem> history)
        {
            if (history.Any())
            {
                this.versionHistoryTableView.Source = new VersionHistoryTableViewSource(history.ToList(), this.VersionHistorySelected);
                this.versionHistoryTableView.ReloadData();
                this.versionHistoryTableViewHeightConstraint.Constant = 
                    PackageDetailScreen.GetTableViewCellsHeight(this.versionHistoryTableView, 0);
                this.versionHistoryTableView.Hidden = false;
            }
        }

        private static void AutoSizeTableViewHeight(UITableView tableView)
        {
            var frame = tableView.Frame;
            var height = GetTableViewCellsHeight(tableView, 0);
            tableView.Frame = new RectangleF(frame.Left, frame.Top, frame.Width, height);
        }

        private static float GetTableViewCellsHeight(UITableView tableView, int sectionIndex)
        {
            tableView.LayoutIfNeeded();
            return tableView.ContentSize.Height;
        }

        /// <summary>
        /// Display an overlay with a loading message
        /// </summary>
        /// <param name="resourceKey">Resource key.</param>
        private void ShowLoadingOverlay(string resourceKey)
        {
            if (this.loadingOverlay == null) 
            {
                this.loadingOverlay = new LoadingOverlay(this.View.Frame, Resources.GetString(resourceKey));
                View.Add(this.loadingOverlay);
            }
        }

        /// <summary>
        /// Hide the loading overlay, if it is shown
        /// </summary>
        private void HideLoadingOverlay()
        {
            if (this.loadingOverlay != null) 
            {
                this.loadingOverlay.Hide();
                this.loadingOverlay.Dispose();
                this.loadingOverlay = null;
            }
        }


        private void DependencySelected(PackageDependency dependency)
        {
            if (this.networkChecker.ValidateNetworkConnectivity()) 
            { 
                this.ShowLoadingOverlay(Resource.String.wait_loading);
                try
                {
                    this.NavigationController.PushViewController(new PackageDetailScreen(null, dependency.Title), true);
                }
                finally
                {
                    this.HideLoadingOverlay();
                }
            }
        }

        private void VersionHistorySelected(HistoryItem historyItem)
        {
            if (this.networkChecker.ValidateNetworkConnectivity()) 
            { 
                this.ShowLoadingOverlay(Resource.String.wait_loading);
                try
                {
                    this.NavigationController.PushViewController(new PackageDetailScreen(historyItem.Id, null), true);
                }
                finally
                {
                    this.HideLoadingOverlay();
                }
            }
        }

    }
}