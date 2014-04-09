using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using NuGetSearch.Common;

namespace NuGetSearch.Android
{
	/// <summary>
	/// Search activity.
	/// </summary>
	[Activity(Label = "NuGet Search", WindowSoftInputMode = SoftInput.StateVisible, ConfigurationChanges = ConfigChanges.Orientation)]
	public class SearchActivity : Activity
	{
		private INuGetGalleryClient nugetGalleryClient;
		private INetworkChecker networkChecker;
		private bool includePrerelease = false;
		private string orderBy = "DownloadCount desc";
		private string searchTerm = null;
		
		public SearchActivity() : base()
		{
			this.nugetGalleryClient = new NuGetGalleryClient("http://www.nuget.org", new NetworkProvider());
			this.networkChecker = new AndroidNetworkChecker(this);
		}
		
		/// <summary>
		/// Adds menu items to the menu for this activity
		/// </summary>
		/// <returns></returns>
		/// <param name="menu">The menu for this activity</param>
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			this.MenuInflater.Inflate(Resource.Menu.SearchMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}
		
		/// <summary>
		/// Handles menu item selection events
		/// </summary>
		/// <returns></returns>
		/// <param name="item">The selected menu item</param>
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.newSearchMenu:
					var searchIntent = new Intent(this, typeof(SearchActivity));
					searchIntent.AddFlags(ActivityFlags.ClearTop);
					this.StartActivity(searchIntent);
					break;

				case Resource.Id.stableOnlyMenu:
					item.SetChecked(true);
					this.includePrerelease = false;
					this.SearchAsync();
					break;

				case Resource.Id.includePrereleaseMenu:
					item.SetChecked(true);
					this.includePrerelease = true;
					this.SearchAsync();
					break;
				
				case Resource.Id.sortByTitleMenu:
					item.SetChecked(true);
					this.orderBy = "Title";
					this.SearchAsync();
					break;
				
				case Resource.Id.sortByPopularityMenu:
					item.SetChecked(true);
					this.orderBy = "DownloadCount desc";
					this.SearchAsync();
					break;
				
				case Resource.Id.sortByDateAddedMenu:
					item.SetChecked(true);
					this.orderBy = "Created desc";				
					this.SearchAsync();
					break;
			}
			
			return base.OnOptionsItemSelected(item);
		}

		/// <summary>
		/// Activity Creation
		/// </summary>
		/// <param name="bundle"></param>
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			this.SetContentView(Resource.Layout.Search);
			
			// If the intent contains a searcTerm value, then initialize the value of searchTerm
			string extraSearchTerm = Intent.GetStringExtra("searchTerm");
			if (!string.IsNullOrEmpty(extraSearchTerm))
			{
				this.searchTerm = extraSearchTerm;
			}
			
			// If the intent contains an includePrerelease value, then initialize the value of includePrerelease
			string extraIncludePrerelease = Intent.GetStringExtra("includePrerelease");
			if (!string.IsNullOrEmpty(extraIncludePrerelease))
			{
				this.includePrerelease = bool.Parse(extraIncludePrerelease);
			}
			
			// If the intent contains an orderBy value, then initialize the value of orderBy
			string extraOrderBy = Intent.GetStringExtra("orderBy");
			if (!string.IsNullOrEmpty(extraOrderBy))
			{
				this.orderBy = extraOrderBy;
			}
			
			// If there is a searchTerm initialized, then immediately perform the search, 
			// otherwise, display the download/packagecount stats
			if (!string.IsNullOrEmpty(this.searchTerm))
			{
				this.SearchAsync();
			}
			else
			{
				this.DisplayNuGetStatisticsAsync();
			}

			// Check for network connectivity and display a message if there is none
			this.DisplayNetworkConnectivityMessage(this.networkChecker.HasNetworkConnectivity());			
			
			// Set the initial search term text, if specified, and set focus to the textbox by default
			var searchTermEditText = this.FindViewById<EditText>(Resource.Id.searchTerm);
			searchTermEditText.Text = this.searchTerm;
			searchTermEditText.RequestFocus();
		}
		
		/// <summary>
		/// Resume
		/// </summary>
		protected override void OnResume()
		{
			// Wire up the search button click event handler
			this.FindViewById<ImageButton>(Resource.Id.searchButton).Click += this.SearchButton_Click;
			
			// Wire up the edit text editor action event handler
			this.FindViewById<EditText>(Resource.Id.searchTerm).EditorAction += this.SearchTermEditText_EditorAction;
			
			base.OnResume();
		}
		
		/// <summary>
		/// Pause
		/// </summary>
		protected override void OnPause()
		{
			// Remove the search button click event handler
			this.FindViewById<ImageButton>(Resource.Id.searchButton).Click -= this.SearchButton_Click;
			
			// Remove the edit text editor action event handler
			this.FindViewById<EditText>(Resource.Id.searchTerm).EditorAction -= this.SearchTermEditText_EditorAction;
			
			base.OnPause();			
		}
		
		/// <summary>
		/// Shows or hides a message that indicates whether there is network connectivity or not
		/// </summary>
		/// <param name="connected"><c>True</c> if the device is connected to a network</param>
		private void DisplayNetworkConnectivityMessage(bool connected)
		{
			FindViewById<TextView>(Resource.Id.noNetwork).Visibility = connected ? ViewStates.Gone : ViewStates.Visible;
		}
		
		/// <summary>
		/// Handle the event when the user clicks the search action of the soft input keyboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SearchTermEditText_EditorAction(object sender, TextView.EditorActionEventArgs e)
		{
			if (e.ActionId == ImeAction.Search)
			{
				this.searchTerm = FindViewById<EditText>(Resource.Id.searchTerm).Text;
				this.SearchAsync();
				e.Handled = true;
			}
		}

		/// <summary>
		/// Handles the event when the user clicks the search button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SearchButton_Click(object sender, EventArgs e)
		{
			this.searchTerm = FindViewById<EditText>(Resource.Id.searchTerm).Text;
			this.SearchAsync();
		}

		/// <summary>
		/// Hides the soft input keyboard
		/// </summary>
		private void HideSoftInput()
		{
			InputMethodManager inputMethodManager = this.GetSystemService(Context.InputMethodService) as InputMethodManager;
			inputMethodManager.HideSoftInputFromWindow(FindViewById<EditText>(Resource.Id.searchTerm).WindowToken, HideSoftInputFlags.None);
		}
		
		/// <summary>
		/// Runs a task that performs the search and displays the results
		/// </summary>
		/// <returns>Task that is performing the search</returns>
		private Task SearchAsync()
		{
			this.HideSoftInput();
			
			// Check if there is network connectivity, an if there is not, then return
			this.DisplayNetworkConnectivityMessage(this.networkChecker.HasNetworkConnectivity());			
			if (!this.networkChecker.ValidateNetworkConnectivity())
			{
				return Task.Run(() => { });
			}
			
			// Show the progress dialog
			ProgressDialog progressDialog = ProgressDialog.Show(this, null, Resources.GetString(Resource.String.wait_searching), true);
			
			return Task.Run(() => 
			{
				try
				{
					// Create a search result adapter containing the search results
					SearchResultAdapter sra = new SearchResultAdapter(
						this, 
						this.searchTerm, 
						this.orderBy, 
						this.includePrerelease);
					
					// Display the search results
					this.RunOnUiThread(() => 
					{
						// Set the result count
						var countTextView = this.FindViewById<TextView>(Resource.Id.searchResultCount);
						countTextView.Text = (sra.TotalCount == 1) ? 
							this.Resources.GetString(Resource.String.search_result_count_1) : 
							string.Format(this.Resources.GetString(Resource.String.search_result_count_format), sra.TotalCount);
							
						// Bind the search result adapter to the listview to display the items in the result
						var listView = this.FindViewById<ListView>(Resource.Id.searchResults);
						listView.Adapter = sra;
						listView.Visibility = ViewStates.Visible;
						
						// Hide the stats layout
						this.FindViewById<LinearLayout>(Resource.Id.statsLayout).Visibility = ViewStates.Gone;
						
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
					this.RunOnUiThread(() => 
					{
						progressDialog.Dismiss();
						progressDialog.Dispose();
					});
				}
			});
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
					
					this.RunOnUiThread(() => 
					{
						// Display the stats
						this.FindViewById<TextView>(Resource.Id.uniquePackages).Text = stats.UniquePackages;
						this.FindViewById<TextView>(Resource.Id.totalPackageDownloads).Text = stats.TotalPackageDownloads;
						this.FindViewById<TextView>(Resource.Id.totalPackages).Text = stats.TotalPackages;
						
						// If search results are not displayed, then show the stats layout
						if (this.FindViewById<ListView>(Resource.Id.searchResults).Visibility != ViewStates.Visible)
						{
							this.FindViewById<LinearLayout>(Resource.Id.statsLayout).Visibility = ViewStates.Visible;
						}
						
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
