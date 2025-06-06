using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace KitapTakipMaui.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private RegisterModel registerModel = new();

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            Debug.WriteLine("RegisterViewModel initialized.");
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            Debug.WriteLine("RegisterAsync started.");
            if (string.IsNullOrEmpty(RegisterModel.Username) || string.IsNullOrEmpty(RegisterModel.Email) ||
                string.IsNullOrEmpty(RegisterModel.Password))
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Tüm alanlar doldurulmalı.", "Tamam");
                Debug.WriteLine("RegisterAsync: Some fields are empty.");
                return;
            }

            try
            {
                Debug.WriteLine($"RegisterModel: Username={RegisterModel.Username}, Email={RegisterModel.Email}, Password={RegisterModel.Password}");
                var (success, message) = await _authService.RegisterAsync(RegisterModel);
                Debug.WriteLine($"RegisterAsync result from AuthService: Success={success}, Message={message}");
                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Başarılı", message, "Tamam");
                    if (Shell.Current != null)
                    {
                        await Shell.Current.GoToAsync("LoginPage"); // Mutlak rota (//) kaldırıldı
                        Debug.WriteLine("Navigated to LoginPage.");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Navigasyon başarısız.", "Tamam");
                        Debug.WriteLine("RegisterAsync: Shell.Current is null.");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", $"Kayıt başarısız: {message}", "Tamam");
                    Debug.WriteLine("RegisterAsync: Registration failed with message: " + message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegisterAsync exception: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert("Hata", $"Kayıt işlemi sırasında hata: {ex.Message}\nInner: {ex.InnerException?.Message}", "Tamam");
            }
        }
    }
}