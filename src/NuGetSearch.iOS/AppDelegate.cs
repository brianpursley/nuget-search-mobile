using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NuGetSearch.Common;

namespace NuGetSearch.IOS
{
    /// <summary>
    /// The UIApplicationDelegate for the application. This class is responsible for launching the
    /// User Interface of the application, as well as listening (and optionally responding) to
    /// application events from iOS.
    /// </summary>
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
        private UIWindow window;
        private SearchScreen searchScreen;

        /// <summary>
        /// This method is invoked when the application has loaded and is ready to run. In this
        /// method you should instantiate the window, load the UI into it and then make the window
        /// visible.
        /// You have 17 seconds to return from this method, or iOS will terminate your application.
        /// </summary>
        /// <returns><c>true</c>, if launching was finisheded, <c>false</c> otherwise.</returns>
        /// <param name="app">App.</param>
        /// <param name="options">Options.</param>
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			this.searchScreen = new SearchScreen();

            var rootNavigationController = new UINavigationController();
            rootNavigationController.PushViewController(this.searchScreen, false);

			this.window = new UIWindow(UIScreen.MainScreen.Bounds);
            this.window.RootViewController = rootNavigationController;
            this.window.MakeKeyAndVisible();
			return true;
		}
	}
}