using Android.App;
using Android.Content;
using Android.Net;
using NuGetSearch.Common;

namespace NuGetSearch.Android
{
	/// <summary>
	/// Android network checker implementation
	/// </summary>
	public class AndroidNetworkChecker : INetworkChecker
	{
		private Context context;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="NuGetSearch.Android.AndroidNetworkChecker"/> class.
		/// </summary>
		/// <param name="context">Context</param>
		public AndroidNetworkChecker(Context context)
		{
			this.context = context;
		}
		
		/// <summary>
		/// Determines whether the device currently has network connectivity.
		/// </summary>
		/// <returns><c>true</c> if the device has network connectivity; otherwise, <c>false</c>.</returns>
		public bool HasNetworkConnectivity()
		{
			var cm = (ConnectivityManager)this.context.GetSystemService(Context.ConnectivityService);
			return cm.ActiveNetworkInfo != null && cm.ActiveNetworkInfo.IsConnected;
		}
		
		/// <summary>
		/// Checks if the device has network connectivity, and if not, displays an alert dialog to the user
		/// </summary>
		/// <returns><c>true</c>, if the device has network connectivity, <c>false</c> otherwise.</returns>
		public bool ValidateNetworkConnectivity()
		{
			if (this.HasNetworkConnectivity())
			{
				return true;
			}
			else
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this.context);
				builder.SetTitle(Resource.String.no_network_title);
				builder.SetMessage(Resource.String.no_network);
				builder.SetPositiveButton(Resource.String.ok, (sender, e) => { });
				builder.Show();
				return false;
			}
		}
	}
}
