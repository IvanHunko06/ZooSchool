using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using System.Net;
using System.Net.Http;
using System.Reflection;
namespace Client
{
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
                    fonts.AddFont("Nunito-Black.ttf", "NunitoBlack");
                    fonts.AddFont("Nunito-BlackItalic.ttf", "NunitoBlackItalic");
                    fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                    fonts.AddFont("Nunito-BoldItalic.ttf", "NunitoBoldItalic");

                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("Client.appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);


            builder.Services.AddSingleton<Services.NetworkService>();

            builder.Services.AddSingleton<AutorizationPage>();
            builder.Services.AddSingleton<LessonPage>();

            return builder.Build();
        }
    }
}
