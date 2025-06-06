using KitapTakipMaui.Services;
using KitapTakipMaui.Services.Interfaces;
using KitapTakipMaui.ViewModels;
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
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // HttpClient ekle (tüm servisler için ortak)
            builder.Services.AddHttpClient();

            // Servisleri ekle
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IBookService, BookService>();

            // ViewModel'leri ekle
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<BookListViewModel>();

            return builder.Build();
        }
    }
}