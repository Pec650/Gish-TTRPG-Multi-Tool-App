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
                // Core Template Fonts
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                // Your Custom Design Typography Mappings
                fonts.AddFont("AveriaLibre-Bold.ttf", "AveriaLibre-Bold.ttf");
                fonts.AddFont("Faustina-Bold.ttf", "Faustina-Bold.ttf");
                fonts.AddFont("Harmattan-Bold.ttf", "Harmattan-Bold.ttf");
                fonts.AddFont("Commissioner-Medium.ttf", "Commissioner-Medium.ttf");
                fonts.AddFont("Commissioner-Bold.ttf", "Commissioner-Bold.ttf");
                fonts.AddFont("Merriweather-Bold.ttf", "Merriweather-Bold.ttf");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}