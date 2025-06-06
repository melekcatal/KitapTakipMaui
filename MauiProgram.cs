using KitapTakipMaui.Services;
using KitapTakipMaui.ViewModels;
using KitapTakipMaui.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace KitapTakipMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>() // App sınıfına IServiceProvider enjekte ediliyor
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Bağımlılık enjeksiyonu yapılandırması
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7220/api/auth/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            return builder.Build();
        }
    }
}