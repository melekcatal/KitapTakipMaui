using KitapTakipMaui.Services.Interfaces;
using KitapTakipMaui.Views;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace KitapTakipMaui
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell()
        {
            Debug.WriteLine("AppShell constructor started.");
            InitializeComponent();

            if (App.Services == null)
            {
                Debug.WriteLine("AppShell: App.Services is null.");
                throw new InvalidOperationException("App.Services is null. Ensure MauiProgram.cs properly initializes App.Services.");
            }

            _authService = App.Services.GetService<IAuthService>();
            if (_authService == null)
            {
                Debug.WriteLine("AppShell: IAuthService could not be resolved.");
                throw new InvalidOperationException("IAuthService could not be resolved. Ensure IAuthService is registered in MauiProgram.cs.");
            }

            // Rotaları kaydet
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(BookListPage), typeof(BookListPage));
            Routing.RegisterRoute(nameof(BookDetailPage), typeof(BookDetailPage));
            Routing.RegisterRoute(nameof(AddBookPage), typeof(AddBookPage));
            Routing.RegisterRoute(nameof(EditBookPage), typeof(EditBookPage));
            Debug.WriteLine("AppShell: Routes registered.");
        }

        protected override async void OnAppearing()
        {
            Debug.WriteLine("AppShell.OnAppearing started.");
            base.OnAppearing();
            await CheckUserLoginStatus();
        }

        private async Task CheckUserLoginStatus()
        {
            Debug.WriteLine("CheckUserLoginStatus started.");
            if (Shell.Current == null)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Shell.Current is null.", "Tamam");
                Debug.WriteLine("CheckUserLoginStatus: Shell.Current is null.");
                return;
            }

            try
            {
                var token = _authService.GetToken();
                Debug.WriteLine($"CheckUserLoginStatus: Token: {token}");
                if (!string.IsNullOrEmpty(token))
                {
                    await Shell.Current.GoToAsync($"//{nameof(BookListPage)}");
                    Debug.WriteLine("Navigated to BookListPage.");
                }
                else
                {
                    // Zaten LoginPage'de olduğumuz için yönlendirme gerekmeyebilir
                    Debug.WriteLine("CheckUserLoginStatus: No token, staying on LoginPage.");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Navigasyon hatası: {ex.Message}", "Tamam");
                Debug.WriteLine($"CheckUserLoginStatus exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}