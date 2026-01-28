using WorkOutPro.Data;
using Microsoft.Extensions.Logging;

namespace WorkOutPro;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Initialize SQLite
        SQLitePCL.Batteries_V2.Init();
        
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register DbContext
        builder.Services.AddSingleton<WorkOutProDbContext>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
