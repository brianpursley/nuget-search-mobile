using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// Resource utility functions
    /// </summary>
    public static class Resources
    {
        /// <summary>
        /// Returns value of the string resource that has the specified key
        /// </summary>
        /// <returns>String resource value</returns>
        /// <param name="key">String resource key</param>
        public static string GetString(string key)
        {
            return NSBundle.MainBundle.LocalizedString(key, null);
        }

        /// <summary>
        /// Returns the image resource that has the specified name
        /// </summary>
        /// <returns>Image resource</returns>
        /// <param name="name">String resource name</param>
        public static UIImage GetImage(string name)
        {
            return UIImage.FromBundle(name);
        }
    }
}