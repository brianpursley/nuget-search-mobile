using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using NuGetSearch.Common;

namespace NuGetSearch.Android
{
	/// <summary>
	/// Search result adapter.
	/// </summary>
	public class SearchResultAdapter : BaseAdapter<SearchResultItem>
	{
		private const int InitialBatchSize = 50;
		private const int SubsequentBatchSize = 50;

		private Activity context;
		private INuGetGalleryClient nugetGalleryClient;
		private INetworkChecker networkChecker;
		private string searchTerm;
		private string orderBy;
		private bool includePrerelease;
		private int totalCount = 0;
		private List<SearchResultItem> items = new List<SearchResultItem>(InitialBatchSize + SubsequentBatchSize);

		/// <summary>
		/// Initializes a new instance of the <see cref="NuGetSearch.Android.SearchResultAdapter"/> class
		/// </summary>
		/// <param name="context">The parent context</param>
		/// <param name="searchTerm">The search term entered by the user</param>
		/// <param name="orderBy">A string indicating how the results should be sorted</param>
		/// <param name="includePrerelease">Boolean indicating whether prerelease packages should be included in the result</param>
		public SearchResultAdapter(
				Activity context, 
				string searchTerm, 
				string orderBy, 
				bool includePrerelease) : base() 
		{
			this.context = context;
			this.searchTerm = searchTerm;
			this.orderBy = orderBy;
			this.includePrerelease = includePrerelease;
			
			this.nugetGalleryClient = new NuGetGalleryClient("http://www.nuget.org", new NetworkProvider());
			this.networkChecker = new AndroidNetworkChecker(context);

			this.LoadSearchResultItems(InitialBatchSize);
		}
		
		/// <summary>
		/// Gets the view type count.  There are two view types used by this adapter:
		///   0 = Normal list item
		///   1 = The "loading More..." list item
		/// </summary>
		/// <value>The number of view types used by this adapter</value>
		public override int ViewTypeCount 
		{
			get 
			{
				return 2; 
			}
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
		/// Gets the count of results that have been loaded in the adapter.
		/// If not all of the results have been loaded, then it adds one, so that
		/// the "Loading More..." item can be displayed at the end.
		/// </summary>
		/// <value>The number of items contained in the adapter</value>
		public override int Count 
		{
			get 
			{ 
				return this.HasMore ? this.items.Count + 1 : this.totalCount;
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

		/// <summary>
		/// Gets the <see cref="SearchResultItem"/> with the specified position.
		/// </summary>
		/// <returns>The SearchResultItem at the specified position</returns>
		/// <param name="position"></param>
		public override SearchResultItem this[int position] 
		{  
			get 
			{ 
				return this.IsLoadingMorePosition(position) ? null : this.items[position];
			}
		}

		/// <summary>
		/// Gets the item ID at the specified position.  This adapter simply returns the position as the ID.
		/// </summary>
		/// <returns>The item ID</returns>
		/// <param name="position"></param>
		public override long GetItemId(int position)
		{
			return position;
		}

		/// <summary>
		/// Gets the type of item view at the specified position
		/// </summary>
		/// <returns>The item view type</returns>
		/// <param name="position"></param>
		public override int GetItemViewType(int position)
		{
			return this.IsLoadingMorePosition(position) ? 1 : 0;
		}
		
		/// <summary>
		/// Gets the view to use to display the item at the specified position
		/// </summary>
		/// <returns>A view for displaying the item at the specified position</returns>
		/// <param name="position"></param>
		/// <param name="convertView"></param>
		/// <param name="parent"></param>
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			if (this.IsLoadingMorePosition(position))
			{
				// Check if there is network connectivity, and if so, load more items
				if (this.networkChecker.HasNetworkConnectivity())
				{
					Task.Run(() => { this.EnsurePositionLoaded(position + SubsequentBatchSize); });
				}
				
				// Re-use the view that was passed in the convertView argument, or inflate a new view if it was null
				View view = convertView;
				if (view == null)
				{
					view = this.context.LayoutInflater.Inflate(Resource.Layout.SearchResultLoadingItem, parent, false);
				}

				return view;
			}
			else 
			{
				// Get the item at this position
				var item = this.items[position];
				
				// Re-use the view that was passed in the convertView argument, or inflate a new view if it was null
				View view = convertView;
				if (view == null)
				{
					view = this.context.LayoutInflater.Inflate(Resource.Layout.SearchResultItem, parent, false);
					view.Click += this.View_Click;
				}
				
				// If the view was re-used, then check the tag... 
				// If it is different, then we need to set the title, description, and icon to the new values
				if (view.Tag == null || view.Tag.ToString() != item.Id)
				{
					view.Tag = item.Id;
					view.FindViewById<TextView>(Resource.Id.title).Text = item.DisplayTitle;
					view.FindViewById<TextView>(Resource.Id.description).Text = item.Description;
					
					if (item.UseDefaultIcon)
					{
						var iconImageView = view.FindViewById<ImageView>(Resource.Id.icon);
						SearchResultAdapter.SetDefaultIconImage(iconImageView);
					}
				}
	
				// If not using the default icon, then set the icon image
				if (!item.UseDefaultIcon)
				{
					var iconImageView = view.FindViewById<ImageView>(Resource.Id.icon);
					SearchResultAdapter.SetIconImage(iconImageView, item.IconUrl);
				}

				return view;
			}
		}

		/// <summary>
		/// Sets the default icon image for an image view
		/// </summary>
		/// <param name="iconImageView">Image view to set the image of</param>
		private static void SetDefaultIconImage(ImageView iconImageView)
		{
			// If the tag is set already and equals "default", then the default image has already been set, so just return
			if (iconImageView.Tag != null && iconImageView.Tag.ToString() == "default")
			{
				return;
			}
			
			// Set the image using the default icon from resources
			iconImageView.SetImageResource(Resource.Drawable.packageDefaultIconSmall);
			
			// Set the tag so we can quickly tell whether this image view has already had the default image set or not
			iconImageView.Tag = "default";
		}

		/// <summary>
		/// Ensures the result item at the specified position has been loaded, and if not, loads a new batch of results
		/// </summary>
		/// <param name="position"></param>
		private void EnsurePositionLoaded(int position)
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
				this.LoadSearchResultItems(position + SubsequentBatchSize);
			}
		}

		/// <summary>
		/// Loads search results into the adapter up to a specified position
		/// </summary>
		/// <param name="toPosition"></param>
		private void LoadSearchResultItems(int toPosition)
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

			this.context.RunOnUiThread(() => 
			{ 
				// Only set the total count if this is the first time results are returned (the total count is not there in subsequent requests)
				if (isFirstLoad)
				{
					this.totalCount = searchResult.Count;
				}
				
				// Add the resulting items to the items list
				this.items.AddRange(searchResult.Items);
				this.NotifyDataSetChanged(); 
			});
			
			// Tell the icon manager to make sure it has a bitmap for the icons, and if not, to start loading them
			AndroidIconManager.Current.Load(
				searchResult.Items.Select(x => x.IconUrl).Distinct(),
				x => { this.context.RunOnUiThread(() => { this.NotifyDataSetChanged(); }); });
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
		/// Sets the image of the specified ImageView using the specified URL
		/// </summary>
		/// <param name="iconImageView">Image view to set the image of</param>
		/// <param name="url">URL of the image to use</param>
		private static void SetIconImage(ImageView iconImageView, string url)
		{
			// If the tag is set already and equals the url, then the image has already been set, so just return
			if (iconImageView.Tag != null && iconImageView.Tag.ToString() == url)
			{
				return;
			}
			
			// Check whether the icon manager has loaded the image yet or not
			if (AndroidIconManager.Current.IsLoaded(url))
			{
				// If the image has been loaded, then get it from the icon manager and set it on the image view
				Bitmap icon = AndroidIconManager.Current.GetIcon(url);
				if (icon == null)
				{
					SearchResultAdapter.SetDefaultIconImage(iconImageView);
				}
				else
				{
					try
					{
						iconImageView.SetImageBitmap(icon);
					}
					catch
					{
						SearchResultAdapter.SetDefaultIconImage(iconImageView);
					}
				}
				
				// Set the tag so we can quickly tell whether this image view has already had its image set or not
				iconImageView.Tag = url;
			}
			else
			{
				// If the image has not yet been loaded, set the default icon for now.
				// It will be set to the real image later, after it has been loaded.
				SearchResultAdapter.SetDefaultIconImage(iconImageView);
			}
		}

		/// <summary>
		/// Click event handler for the view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void View_Click(object sender, EventArgs e)
		{
			// Make sure there is network connectivity
			if (!this.networkChecker.ValidateNetworkConnectivity()) 
			{ 
				return; 
			}
			
			// Start the Package Detail Activity
			View view = sender as View;
			var packageDetailIntent = new Intent(this.context, typeof(PackageDetailActivity));
			packageDetailIntent.PutExtra("id", view.Tag.ToString());
			this.context.StartActivity(packageDetailIntent);
		}
	}
}
