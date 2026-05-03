using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace ToDoList;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Window != null)
        {
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#D4956A"));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var uiOptions = Window.DecorView.SystemUiVisibility;
                uiOptions &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;
                Window.DecorView.SystemUiVisibility = uiOptions;
            }
        }
    }
}