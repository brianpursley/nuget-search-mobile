using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Search Result Table View Source implementation
    /// </summary>
    public class SearchResultTableViewSource : UITableViewSource
    {
        private const int InitialBatchSize = 50;
        private const int SubsequentBatchSize = 50;

        private const int IconWidth = 50;
        private const int IconHeight = 50;

        private const string SearchResultCellIdentifier = "1";
        private const string LoadingMoreCellIdentifier = "2";

        private INuGetGalleryClient nugetGalleryClient;
        private INetworkChecker networkChecker;

        private string searchTerm;
        private string orderBy;
        private bool includePrerelease;

        private int totalCount = 0;
        private List<SearchResultItem> items = new List<SearchResultItem>(InitialBatchSize + SubsequentBatchSize);

        private RowSelectedDelegate rowSelectedCallback;
        private RowChangedDelegate rowChangedCallback;

        public delegate void RowSelectedDelegate(SearchResultItem item);
        public delegate void RowChangedDelegate(NSIndexPath indexPath);

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.IOS.SearchResultTableViewSource"/> class.
        /// </summary>
        /// <param name="searchTerm">Search term.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="includePrerelease">If set to <c>true</c> include prerelease.</param>
        public SearchResultTableViewSource(
            string searchTerm, 
            string orderBy, 
            bool includePrerelease,
            RowSelectedDelegate rowSelectedCallback,
            RowChangedDelegate rowChangedCallback) 
            : base()
        {           
            this.searchTerm = searchTerm;
            this.orderBy = orderBy;
            this.includePrerelease = includePrerelease;
            this.rowSelectedCallback = rowSelectedCallback;
            this.rowChangedCallback = rowChangedCallback;

            this.nugetGalleryClient = new NuGetGalleryClient("http://www.nuget.org", new NetworkProvider());
            this.networkChecker = new IOSNetworkChecker();

            this.LoadSearchResultItems(InitialBatchSize, null);
        }

        /// <summary>
        /// Gets the total count of results reported by the NuGet Gallery server.
        /// </summary>
        /// <value>The number of results available to be retrieved</value>
        public int TotalCount
        {
            get 
            {
                return this.totalCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has more results that have not yet been retrieved from the server.
        /// </summary>
        /// <value><c>true</c> if this instance has more results; otherwise, <c>false</c>.</value>
        private bool HasMore
        {
            get
            {
                return this.items.Count < this.totalCount;
            }
        }

        public override string TitleForHeader(UITableView tableView, int section)
        {
            return this.totalCount == 1 ?
                Resources.GetString(Resource.String.result_count_1) :
                string.Format(Resources.GetString(Resource.String.result_count_format), this.totalCount);
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return this.HasMore ? this.items.Count + 1 : this.totalCount;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (rowSelectedCallback != null)
            {
                rowSelectedCallback(this.items[indexPath.Row]);
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            var position = indexPath.Row;

            if (this.IsLoadingMorePosition(position))
            {
                // Check if there is network connectivity, and if so, load more items
                if (this.networkChecker.HasNetworkConnectivity())
                {
                    Task.Run(() => { this.EnsurePositionLoaded(position + SubsequentBatchSize, () => { tableView.ReloadData(); }); });
                }

                // Re-use the cell that was passed in the convertView argument, or create a new cell if it was null
                UITableViewCell cell = tableView.DequeueReusableCell(LoadingMoreCellIdentifier)
                   ?? new UITableViewCell(UITableViewCellStyle.Subtitle, LoadingMoreCellIdentifier) 
                        { SelectionStyle = UITableViewCellSelectionStyle.None }; 

                cell.TextLabel.Text = Resources.GetString(Resource.String.wait_loading_more);

                return cell;
            }
            else 
            {
                // Get the item at this position
                var item = this.items[position];  

                // Re-use the cell that was passed in the convertView argument, or create a new cell if it was null
                SearchResultTableViewCell cell = tableView.DequeueReusableCell(SearchResultCellIdentifier) as SearchResultTableViewCell
                                                 ?? new SearchResultTableViewCell(SearchResultCellIdentifier);

                cell.TextLabel.Text = item.DisplayTitle;
                cell.DetailTextLabel.Text = item.Description;
                SearchResultTableViewSource.SetCellImageViewFrameSize(cell);

                if (item.UseDefaultIcon)
                {
                    SearchResultTableViewSource.SetDefaultIconImage(cell);
                }
                else
                {
                    SearchResultTableViewSource.SetIconImage(cell, item.IconUrl);
                }

                return cell;
            }
        }
            
        private static void SetCellImageViewFrameSize(UITableViewCell cell)
        {
            var frame = cell.ImageView.Frame;
            frame.Width = IconWidth;
            frame.Height = IconHeight;
            cell.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }

        /// <summary>
        /// Sets the default icon image for an image view
        /// </summary>
        /// <param name="cell">Cell to set the image of</param>
        private static void SetDefaultIconImage(UITableViewCell cell)
        {
            cell.ImageView.Image = Resources.GetImage(Resource.Image.packageDefaultIconSmall_png);
        }

        /// <summary>
        /// Sets the image of the specified cell using the specified URL
        /// </summary>
        /// <param name="cell">Cell to set the image of</param>
        /// <param name="url">URL of the image to use</param>
        private static void SetIconImage(UITableViewCell cell, string url)
        {
            // Check whether the icon manager has loaded the image yet or not
            if (IOSIconManager.Current.IsLoaded(url))
            {
                // If the image has been loaded, then get it from the icon manager and set it on the image view
                var icon = IOSIconManager.Current.GetIcon(url);
                if (icon == null)
                {
                    SearchResultTableViewSource.SetDefaultIconImage(cell);
                }
                else
                {
                    try
                    {
                        cell.ImageView.Image = icon;
                    }
                    catch
                    {
                        SearchResultTableViewSource.SetDefaultIconImage(cell);
                    }
                }
            }
            else
            {
                // If the image has not yet been loaded, set the default icon for now.
                // It will be set to the real image later, after it has been loaded.
                SearchResultTableViewSource.SetDefaultIconImage(cell);
            }
        }

        /// <summary>
        /// Returns true if the specified position is the "Loading More..." position
        /// </summary>
        /// <returns><c>true</c> if the specified position is the "Loading More..." position; otherwise, <c>false</c>.</returns>
        /// <param name="position"></param>
        private bool IsLoadingMorePosition(int position)
        {
            return position >= this.items.Count;
        }

        /// <summary>
        /// Ensures the result item at the specified position has been loaded, and if not, loads a new batch of results
        /// </summary>
        /// <param name="position"></param>
        /// <param name="action"></param> 
        private void EnsurePositionLoaded(int position, Action action)
        {
            // If this position is already loaded, then return immediately
            if (!this.IsLoadingMorePosition(position)) 
            { 
                return; 
            }

            lock (this.items)
            {
                // Since we might have been blocked, check again if the position is already loaded, and if so, return
                if (!this.IsLoadingMorePosition(position)) 
                { 
                    return; 
                }

                // Load the requested position PLUS a batch of additional results
                this.LoadSearchResultItems(position + SubsequentBatchSize, action);
            }
        }

        /// <summary>
        /// Loads search results into the adapter up to a specified position
        /// </summary>
        /// <param name="toPosition"></param>
        /// <param name="action"></param> 
        private void LoadSearchResultItems(int toPosition, Action action)
        {
            // Determine if this is the first load
            bool isFirstLoad = this.items.Count == 0;

            // Figure out how many rows to skip and how many to load
            int rowsToSkip = isFirstLoad ? 0 : this.items.Count;
            int rowsToLoad = toPosition - rowsToSkip;

            // Query the remote NuGet gallery server for results
            var searchResult = this.nugetGalleryClient
                .SearchAsync(this.searchTerm, this.orderBy, this.includePrerelease, rowsToSkip, rowsToLoad, isFirstLoad)
                .Result;

            this.InvokeOnMainThread(() =>
            {
                // Only set the total count if this is the first time results are returned (the total count is not there in subsequent requests)
                if (isFirstLoad)
                {
                    this.totalCount = searchResult.Count;
                }

                // Add the resulting items to the items list
                this.items.AddRange(searchResult.Items);
               
                if (action != null)
                {
                    action();
                }
            });

            // Tell the icon manager to make sure it has a bitmap for the icons, and if not, to start loading them

            IOSIconManager.Current.Load(
                searchResult.Items.Select(x => x.IconUrl).Distinct(),
                x => 
            { 
                if (this.rowChangedCallback != null)
                {
                    for (int i = 0; i < this.items.Count; i++)
                    {
                        if (this.items[i].IconUrl == x)
                        {
                            this.rowChangedCallback(NSIndexPath.FromItemSection(i, 0));
                        }
                    }
                }
            });


        }
    }
}