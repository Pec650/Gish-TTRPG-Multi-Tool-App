using Microsoft.Extensions.Logging;

namespace Gish;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("AveriaLibre-Bold.ttf", "AveriaLibreBold");
                fonts.AddFont("Harmattan-Bold.ttf", "HarmattanBold");
                fonts.AddFont("Faustina-Bold.ttf", "FaustinaBold");
                fonts.AddFont("Commissioner-Regular.ttf", "Commissioner");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
            // iOS Editor doesn't have a BorderStyle property like Entry/Picker, 
            // but it has a Layer border or background we might need to reset if styled.
            // By default, iOS UITextView has no border/underline, but you can explicitly clear it:
            handler.PlatformView.Layer.BorderWidth = 0;
#endif
        });

        return builder.Build();
    }
}