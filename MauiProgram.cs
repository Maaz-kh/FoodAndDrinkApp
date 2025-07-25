using Microsoft.Extensions.Logging;
using FoodAndDrinkApp.Services;
using System.IO;
using CommunityToolkit.Maui;

namespace FoodAndDrinkApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // 🔗 Define database path
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "recipes.db");

            return builder.Build();
        }
    }
}
