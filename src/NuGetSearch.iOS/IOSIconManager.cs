using System;
using System.IO;
using MonoTouch.CoreImage;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// iOS Icon Manager
    /// </summary>
    public class IOSIconManager : IconManager<UIImage>
    {
        public static readonly IOSIconManager Current = new IOSIconManager(new NetworkProvider());

        private INetworkProvider networkProvider;

        // Used to single-thread icon image loading, to avoid consuming all resources when loading many icons at once
        private object loadLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetSearch.IOS.IOSIconManager"/> class.
        /// </summary>
        /// <param name="networkProvider">Network provider</param>
        private IOSIconManager(INetworkProvider networkProvider) : base(2, 3000)
        {
            this.networkProvider = networkProvider;
        }

        /// <summary>
        /// Loads the icon image from the specified url
        /// </summary>
        /// <returns>image for the icon</returns>
        /// <param name="url">URL of the icon to load</param>
        protected override UIImage LoadIcon(string url)
        {
            // Get the image stream
            using (Stream s = this.networkProvider.GetStreamAsync(url).Result)
            {
                // Only allow one image to be loaded at a time, to avoid overwhelming the device
                lock (this.loadLock)
                {
                    using (var data = NSData.FromStream(s))
                    {
                        // Return the image
                        return UIImage.LoadFromData(data);
                    }
                }
            }
        }
    }
}