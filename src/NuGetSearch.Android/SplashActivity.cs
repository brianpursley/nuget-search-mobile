using Android.App;
using Android.OS;

namespace NuGetSearch.Android
{
	/// <summary>
	/// Splash activity.
	/// </summary>
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
    	/// <summary>
    	/// This activity, upon creation, simple starts the Search activity
    	/// </summary>
    	/// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.StartActivity(typeof(SearchActivity));
        }
    }
}
