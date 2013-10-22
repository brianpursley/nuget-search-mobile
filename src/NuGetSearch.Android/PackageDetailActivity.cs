using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using NuGetSearch.Common;

namespace NuGetSearch.Android
{
	/// <summary>
	/// Package detail activity.
	/// </summary>
	[Activity(Label = "NuGet Package Detail", ConfigurationChanges = ConfigChanges.Orientation)]			
	public class PackageDetailActivity : Activity
	{
		private INuGetGalleryClient nugetGalleryClient;
		private INetworkChecker networkChecker;

		public PackageDetailActivity() : base() 
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
			this.MenuInflater.Inflate(Resource.Menu.PackageDetailMenu, menu);
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
			this.SetContentView(Resource.Layout.PackageDetail);
			
			// Get the id and title values from the intent
			string extraId = Intent.GetStringExtra("id");
			string extraTitle = Intent.GetStringExtra("title");
			
			// Display the package details using the specified id and title
			this.DisplayPackageDetailsAsync(extraId, extraTitle);
		}
		
		/// <summary>
		/// Displays the package details
		/// </summary>
		/// <returns>Task</returns>
		/// <param name="packageUrl">The url of the package to display</param>
		/// <param name="title">The title of the package to display</param>
		private Task DisplayPackageDetailsAsync(string packageUrl, string title)
		{
			// Check if there is network connectivity, an if there is not, then return
			if (!this.networkChecker.ValidateNetworkConnectivity()) 
			{ 
				return Task.Run(() => { }); 
			}	
				
			// Show the progress dialog
			ProgressDialog progressDialog = ProgressDialog.Show(this, null, Resources.GetString(Resource.String.wait_loading), true);

			return Task.Run(() => 
			{
				try
				{
					// If the package url was not specified, then retrieve the package url of the latest version based on the title
					if (string.IsNullOrEmpty(packageUrl))
					{
						packageUrl = nugetGalleryClient.GetPackageLatestIdAsync(title).Result;
					}
					
					// Retrieve package details
					var pd = nugetGalleryClient.GetPackageDetailAsync(packageUrl).Result;
					
					// Display the package details
					RunOnUiThread(() => { this.DisplayPackageDetails(pd); });
					
					// Retrieve the version history
					var history = nugetGalleryClient.GetPackageHistoryAsync(pd.Title).Result;
					
					// Display the version history
					RunOnUiThread(() => { this.DisplayVersionHistory(pd.Id, history); });
					}
				finally
				{
					// Hide the progress dialog
					RunOnUiThread(() => 
					{
						progressDialog.Dismiss();
						progressDialog.Dispose();
					});
				}
			});
		}

		/// <summary>
		/// Displays the specified icon
		/// </summary>
		/// <param name="url">The url of the icon to display</param>
		private void DisplayIcon(string url)
		{
			var iconImageView = FindViewById<ImageView>(Resource.Id.icon);
			
			// Check if the icon manager has finished loading the bitmap or not
			if (AndroidIconManager.Current.IsLoaded(url))
			{
				// If the icon manager has loaded the bitmap, then get it
				Bitmap icon = AndroidIconManager.Current.GetIcon(url);
				if (icon == null)
				{
					// If the bitmap failed to load, then set the default icon image
					iconImageView.SetImageResource(Resource.Drawable.packageDefaultIcon);
				}
				else
				{
					// If the bitmap was loaded successfully, then set the bitmap image
					try
					{
						iconImageView.SetImageBitmap(icon);
					}
					catch
					{
						iconImageView.SetImageResource(Resource.Drawable.packageDefaultIcon);
					}
				}
			}
			else
			{
				// If the icon manager has not finished loading the bitmap, then set the default icon image for now
				iconImageView.SetImageResource(Resource.Drawable.packageDefaultIcon);
			}
		}

		/// <summary>
		/// Displays the package details
		/// </summary>
		/// <param name="pd">The package details to display</param>
		private void DisplayPackageDetails(PackageDetail pd)
		{
			// Set default icon image until the real icon can be loaded
			FindViewById<ImageView>(Resource.Id.icon).SetImageResource(Resource.Drawable.packageDefaultIcon);
		
			// Tell the icon manager to load the icon
			AndroidIconManager.Current.Load(pd.IconUrl, x => { RunOnUiThread(() => { this.DisplayIcon(x); }); });
					
			// Populate the header fields (title and version)
			FindViewById<TextView>(Resource.Id.title).Text = pd.DisplayTitle;
			FindViewById<TextView>(Resource.Id.version).Text = pd.Version;
			
			// Show the header layout
			FindViewById<RelativeLayout>(Resource.Id.headerLayout).Visibility = ViewStates.Visible;

			// Check if this package is prerelease and if so, show the prerelease indicator
		    FindViewById<TextView>(Resource.Id.prerelease).Visibility = pd.IsPrerelease ? ViewStates.Visible : ViewStates.Gone;
		    
		    // Populate the description
			FindViewById<TextView>(Resource.Id.description).Text = pd.Description;
			
			// Display project site link, license link, and dependencies list
			this.DisplayProjectSite(pd);
			this.DisplayLicense(pd);
			this.DisplayAuthors(pd);		
			this.DisplayDependencies(pd);
			
			// Show the detail layout
			FindViewById<ScrollView>(Resource.Id.detailLayout).Visibility = ViewStates.Visible;
		}
		
		/// <summary>
		/// Displays the project site link from the specified package details
		/// </summary>
		/// <param name="pd"></param>
		private void DisplayProjectSite(PackageDetail pd)
		{
			var projectSiteCaptionTextView = FindViewById<TextView>(Resource.Id.projectSiteCaption);
			var projectSiteTextView = FindViewById<TextView>(Resource.Id.projectSite);

			// Show/hide the project site caption
			projectSiteCaptionTextView.Visibility = string.IsNullOrEmpty(pd.ProjectUrl) ? ViewStates.Gone : ViewStates.Visible;
		
			// Populate and show/hide the project site
			projectSiteTextView.Text = pd.ProjectUrl;
			projectSiteTextView.Visibility = string.IsNullOrEmpty(pd.ProjectUrl) ? ViewStates.Gone : ViewStates.Visible;
		}
		
		/// <summary>
		/// Displays the license link from the specified package details
		/// </summary>
		/// <param name="pd"></param>
		private void DisplayLicense(PackageDetail pd)
		{
			var licenseCaptionTextView = FindViewById<TextView>(Resource.Id.licenseCaption);
			var licenseTextView = FindViewById<TextView>(Resource.Id.license);

			// Show/hide the license caption
			licenseCaptionTextView.Visibility = string.IsNullOrEmpty(pd.LicenseUrl) ? ViewStates.Gone : ViewStates.Visible;
			
			// Populate and show/hide the license
			licenseTextView.Text = pd.LicenseUrl;
			licenseTextView.Visibility = string.IsNullOrEmpty(pd.LicenseUrl) ? ViewStates.Gone : ViewStates.Visible;
		}
		
		/// <summary>
		/// Displays the authors for the specified package details
		/// </summary>
		/// <param name="pd"></param>
		private void DisplayAuthors(PackageDetail pd)
		{
			var authorsCaptionTextView = FindViewById<TextView>(Resource.Id.authorsCaption);
			var authorsTextView = FindViewById<TextView>(Resource.Id.authors);

			// Populate and show/hide the authors caption and authors
			if (pd.Authors.Any())
			{
				authorsCaptionTextView.Visibility = ViewStates.Visible;
				authorsTextView.Text = string.Join(", ", pd.Authors.ToArray());
				authorsTextView.Visibility = ViewStates.Visible;
			}
			else 
			{
				authorsCaptionTextView.Visibility = ViewStates.Gone;
				authorsTextView.Text = string.Empty;
				authorsTextView.Visibility = ViewStates.Gone;
			}
		}
		
		/// <summary>
		/// Displays the dependencies from the specified package details
		/// </summary>
		/// <param name="pd"></param>
		private void DisplayDependencies(PackageDetail pd)
		{
			var dependenciesCaptionTextView = FindViewById<TextView>(Resource.Id.dependenciesCaption);
			var dependenciesLayout = FindViewById<LinearLayout>(Resource.Id.dependencies);
			
			// Check if there are any dependencies
			if (pd.Dependencies.Any())
			{
				// If there are dependencies, then create a DependencyAdapter to display them
				var da = new DependencyAdapter(this, pd.Dependencies);
				for (int i = 0; i < da.Count; i++)
				{
					dependenciesLayout.AddView(da.GetView(i, null, null));
				}

				// Show the dependencies caption and dependencies
				dependenciesCaptionTextView.Visibility = ViewStates.Visible;
				dependenciesLayout.Visibility = ViewStates.Visible;
			}
			else 
			{
				// If there are no dependencies, then hide the dependencies caption and dependencies
				dependenciesCaptionTextView.Visibility = ViewStates.Gone;
				dependenciesLayout.Visibility = ViewStates.Gone;
			}
		}
		
		/// <summary>
		/// Displays version history from the specified package details
		/// </summary>
		/// <param name="currentId">The id of the version that is currently being displayed</param>
		/// <param name="history">The history items that make up the version history</param>
		private void DisplayVersionHistory(string currentId, IEnumerable<HistoryItem> history)
		{
			var versionHistoryLayout = FindViewById<TableLayout>(Resource.Id.versionHistory);	
			
			// Check if there is any history available
			if (history.Any())
			{
				// Create a table row for each history item
				foreach (var h in history)
				{
					// Create table row
					TableRow tableRow = new TableRow(this) { Tag = h.Id };
					tableRow.SetPadding(0, 10, 0, 10);
					tableRow.SetBackgroundColor(Color.White);
					if (h.Id != currentId)
					{
						tableRow.Click += this.VersionHistoryTableRow_Click;
					}
					
					// Create version text view and add it to the table row
					var versionTextView = new TextView(this) { Text = h.Version };
					versionTextView.SetTextColor(Color.Argb(255, 102, 102, 102));
					versionTextView.SetTypeface(null, (h.Id == currentId) ? TypefaceStyle.Normal : TypefaceStyle.Bold);
					versionTextView.SetPadding(5, 0, 10, 0);
					versionTextView.SetBackgroundColor(Color.White);
		            tableRow.AddView(versionTextView);
		            
		            // Create download count text view and add it to the table row
		            var downloadCountTextView = new TextView(this) { Text = h.VersionDownloadCount.ToString("n0"), Gravity = GravityFlags.Right };
					downloadCountTextView.SetTextColor(Color.Argb(255, 153, 153, 153));
					downloadCountTextView.SetPadding(0, 0, 10, 0);
					downloadCountTextView.SetBackgroundColor(Color.White);
		            tableRow.AddView(downloadCountTextView);
		            
		            // Create last updated text view and add it to the table row
		            var lastUpdatedTextView = new TextView(this) { Text = h.Published.ToShortDateString(), Gravity = GravityFlags.Right };
					lastUpdatedTextView.SetTextColor(Color.Argb(255, 153, 153, 153));
					lastUpdatedTextView.SetPadding(0, 0, 5, 0);
					lastUpdatedTextView.SetBackgroundColor(Color.White);
		            tableRow.AddView(lastUpdatedTextView);
		            
		            // Add the table row to the table layout
		            versionHistoryLayout.AddView(tableRow);

					// Create a "separator line" and add it to the table layout
					View v = new View(this);
					v.LayoutParameters = new TableRow.LayoutParams(ViewGroup.LayoutParams.FillParent, 1);
					v.SetBackgroundColor(Color.Argb(255, 240, 240, 240));
					versionHistoryLayout.AddView(v);
				}
				
				// Show the history layout
				versionHistoryLayout.Visibility = ViewStates.Visible;
			}
			else
			{
				// If there is no history, then hide the history layout
				versionHistoryLayout.Visibility = ViewStates.Gone;
			}
		}

		/// <summary>
		/// Click event handler for the version history table
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VersionHistoryTableRow_Click(object sender, EventArgs e)
		{
			// Check if there is network connectivity, an if there is not, then return
			if (!this.networkChecker.ValidateNetworkConnectivity()) 
			{ 
				return; 
			}

			// Start a new Package Detail Activity for this specific version id
			View view = sender as View;
			var packageDetailIntent = new Intent(this, typeof(PackageDetailActivity));
			packageDetailIntent.PutExtra("id", view.Tag.ToString());
			this.StartActivity(packageDetailIntent);
		}
	}
}
