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

        // Force the underlying native window canvas color to eliminate the white flash
        Window.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.ParseColor("#EBE5C9")));

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            Window.SetDecorFitsSystemWindows(false);
        }
        else
        {
            #pragma warning disable CS0618
            Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
            #pragma warning restore CS0618
        }
    }
}