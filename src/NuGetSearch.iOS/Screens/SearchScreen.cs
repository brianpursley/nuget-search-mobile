using System;
using System.Drawing;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Search Screen
    /// </summary>
	public partial class SearchScreen : UIViewController
	{
        private const bool DefaultIncludePrerelease = true;
        private const string DefaultOrderBy = "DownloadCount desc";

        private INuGetGalleryClient nugetGalleryClient;
		private INetworkChecker networkChecker;
		private string searchTerm = null;
		private LoadingOverlay loadingOverlay = null;

        public SearchScreen()
            : base("SearchScreen", null)
        {
            this.nugetGalleryClient = new NuGetGalleryClient(new NetworkProvider());
            this.networkChecker = new IOSNetworkChecker();
        }

        /// <summary>
        /// Returns whether the app is running as a Phone
        /// </summary>
        /// <value><c>true</c> if user interface idiom is phone; otherwise, <c>false</c>.</value>
        private static bool UserInterfaceIdiomIsPhone 
        {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public override void ViewDidLoad()
		{
            base.ViewDidLoad();

            this.noNetworkLabel.Hidden = true;
            this.uniquePackagesCaptionLabel.Hidden = true;
            this.uniquePackagesLabel.Hidden = true;
            this.totalPackageDownloadsCaptionLabel.Hidden = true;
            this.totalPackageDownloadsLabel.Hidden = true;
            this.totalPackagesCaptionLabel.Hidden = true;
            this.totalPackagesLabel.Hidden = true;

            this.noNetworkLabel.Text = Resources.GetString(Resource.String.no_network);
            this.uniquePackagesCaptionLabel.Text = Resources.GetString(Resource.String.unique_packages);
            this.totalPackageDownloadsCaptionLabel.Text = Resources.GetString(Resource.String.total_package_downloads);
            this.totalPackagesCaptionLabel.Text = Resources.GetString(Resource.String.total_packages);

            this.DisplayNuGetStatisticsAsync();

            this.searchBar.SearchButtonClicked += this.SearchBar_SearchButtonClicked;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(true, true);
        }

        private void SearchBar_SearchButtonClicked(object sender, EventArgs e)
		{
			this.searchTerm = this.searchBar.Text;
			this.SearchAsync();
		}
            
        private void PackageSelected(SearchResultItem searchResultItem)
        {
            if (this.networkChecker.ValidateNetworkConnectivity()) 
            { 
                this.ShowLoadingOverlay(Resource.String.wait_loading);
                try
                {
                    this.NavigationController.PushViewController(new PackageDetailScreen(searchResultItem.Id, null), true);
                }
                finally
                {
                    this.HideLoadingOverlay();
                }
            }
        }

        private void RowChanged(NSIndexPath indexPath)
        {
            this.InvokeOnMainThread(() =>
            {
                this.searchResultsTableView.ReloadRows(
                    new NSIndexPath[] { indexPath }, 
                    UITableViewRowAnimation.None);
            });
        }

		/// <summary>
		/// Runs a task that performs the search and displays the results
		/// </summary>
		/// <returns>Task that is performing the search</returns>
		private Task SearchAsync()
		{
			// Check if there is network connectivity, an if there is not, then return
			this.DisplayNetworkConnectivityMessage(this.networkChecker.HasNetworkConnectivity());			
			if (!this.networkChecker.ValidateNetworkConnectivity())
			{
				return Task.Run(() => { });
			}

            this.searchBar.ResignFirstResponder();

            this.ShowLoadingOverlay(Resource.String.wait_loading);

			return Task.Run(() => 
			{
				try
				{
                    // Create a search result table view source
                    var searchResultTableViewSource = new SearchResultTableViewSource(
                        this.searchTerm, 
                        SearchScreen.DefaultOrderBy, 
                        SearchScreen.DefaultIncludePrerelease,
                        this.PackageSelected,
                        this.RowChanged);
                        
					// Display the search results
					this.InvokeOnMainThread(() => 
					{
                        // Display search results
                        this.searchResultsTableView.Source = searchResultTableViewSource;
                        this.searchResultsTableView.ReloadData();
                        this.searchResultsTableView.Hidden = false;
                        this.searchResultsTableView.ScrollRectToVisible(new RectangleF(0, 0, 1, 1), true);

						// Hide the stats layout
						this.statsView.Hidden = true;

						// Indicate that the network has connectivity
						this.DisplayNetworkConnectivityMessage(true);
					});
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine("SearchAsync Exception: " + ex.Message);
				}
				finally 
				{
					// Hide the progress dialog
					this.InvokeOnMainThread(() => 
					{
						this.HideLoadingOverlay();
					});
				}
			});
		}

        /// <summary>
        /// Shows an overlay with a loading message
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
        /// Hides the loading overlay, if it is shown
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

		/// <summary>
		/// Shows or hides a message that indicates whether there is network connectivity or not
		/// </summary>
		/// <param name="connected"><c>True</c> if the device is connected to a network</param>
		private void DisplayNetworkConnectivityMessage(bool connected)
		{
			this.noNetworkLabel.Hidden = connected;
		}

		/// <summary>
		/// Runs a task that retrieves and displays package count and download stats from the NuGet gallery server
		/// </summary>
		/// <returns>Task that is performing the action</returns>
		private Task DisplayNuGetStatisticsAsync()
		{
			return Task.Factory.StartNew(() => 
				{
					try
					{
						// Retrieve the stats
						var stats = this.nugetGalleryClient.GetNuGetStatisticsAsync().Result;

						this.InvokeOnMainThread(() => 
						{
							// Display the stats
							this.uniquePackagesLabel.Text = stats.UniquePackages;
							this.totalPackageDownloadsLabel.Text = stats.TotalPackageDownloads;
							this.totalPackagesLabel.Text = stats.TotalPackages;

                            this.uniquePackagesCaptionLabel.Hidden = false;
                            this.uniquePackagesLabel.Hidden = false;
                            this.totalPackageDownloadsCaptionLabel.Hidden = false;
                            this.totalPackageDownloadsLabel.Hidden = false;
                            this.totalPackagesCaptionLabel.Hidden = false;
                            this.totalPackagesLabel.Hidden = false;

							this.statsView.Hidden = false;

							// Indicate that the network has connectivity
							this.DisplayNetworkConnectivityMessage(true);
						});
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.WriteLine("DisplayNuGetStatisticsAsync Exception: " + ex.Message);
					}
				});
		}
	}
}