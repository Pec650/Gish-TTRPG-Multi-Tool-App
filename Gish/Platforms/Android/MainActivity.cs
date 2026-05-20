using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Gish;

// FIXED: Changed Maui.Splashtheme to Maui.SplashTheme
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Tells the native Android window manager to draw your layouts behind the system bars
        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            Window.SetDecorFitsSystemWindows(false);
        }
        else
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
            #pragma warning restore CS0618
        }
    }
}