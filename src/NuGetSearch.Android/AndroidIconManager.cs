using System.IO;
using Android.Graphics;
using NuGetSearch.Common;

namespace NuGetSearch.Android
{
	/// <summary>
	/// Android Icon Manager
	/// </summary>
	public class AndroidIconManager : IconManager<Bitmap>
	{
		public static readonly AndroidIconManager Current = new AndroidIconManager(new NetworkProvider());
	
		private INetworkProvider networkProvider;

		// Used to single-thread icon bitmap decoding, to avoid consuming all resources when loading many icons at once
		private object decodeLock = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="NuGetSearch.Android.AndroidIconManager"/> class.
		/// </summary>
		/// <param name="networkProvider">Network provider</param>
		private AndroidIconManager(INetworkProvider networkProvider) : base(2, 3000)
		{
			this.networkProvider = networkProvider;
		}
		
		/// <summary>
		/// Loads the icon bitmap from the specified url
		/// </summary>
		/// <returns>Bitmap for the icon</returns>
		/// <param name="url">URL of the icon to load</param>
		protected override Bitmap LoadIcon(string url)
		{
			// Get the bitmap stream
			using (Stream s = this.networkProvider.GetStreamAsync(url).Result)
			{
				// Only allow one bitmap to be decoded at a time, to avoid overwhelming the device
				lock (this.decodeLock)
				{
					// Return the decoded bitmap
					return BitmapFactory.DecodeStream(s);
				}
			}
		}
	}
}
