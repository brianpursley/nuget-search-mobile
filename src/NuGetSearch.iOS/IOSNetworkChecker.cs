using System;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// iOS Network Checker implementation
    /// </summary>
	public class IOSNetworkChecker : INetworkChecker
	{
		/// <summary>
		/// Determines whether the device currently has network connectivity.
		/// </summary>
		/// <returns><c>true</c> if the device has network connectivity; otherwise, <c>false</c>.</returns>
		public bool HasNetworkConnectivity()
		{
            return true;
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
                using (var alertView = new UIAlertView(
                    Resources.GetString(Resource.String.no_network_title), 
                    Resources.GetString(Resource.String.no_network), 
                    null, 
                    Resources.GetString(Resource.String.ok), 
                    null))
                {
                    alertView.Show();
                }

				return false;
			}
		}
	}
}