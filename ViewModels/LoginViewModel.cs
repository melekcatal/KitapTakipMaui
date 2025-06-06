using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace KitapTakipMaui.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private LoginModel loginModel = new();

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            Debug.WriteLine("LoginViewModel initialized.");
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            Debug.WriteLine("LoginAsync started.");
            if (string.IsNullOrEmpty(LoginModel.Username) || string.IsNullOrEmpty(LoginModel.Password))
            {
                await Application.Current.MainPage.DisplayAlert("Hata", "Kullanıcı adı ve şifre gerekli.", "Tamam");
                Debug.WriteLine("LoginAsync: Username or Password empty.");
                return;
            }

            try
            {
                Debug.WriteLine($"LoginModel: {LoginModel.Username}, {LoginModel.Password}");
                var (success, message, token) = await _authService.LoginAsync(LoginModel);
                Debug.WriteLine($"LoginAsync result: Success={success}, Message={message}, Token={token}");
                if (success)
                {
                    if (Shell.Current == null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Hata", "Shell navigasyonu başlatılamadı.", "Tamam");
                        Debug.WriteLine("LoginAsync: Shell.Current is null.");
                        return;
                    }
                    // Token'ı saklamak için (isteğe bağlı)
                    // await SecureStorage.SetAsync("auth_token", token);
                    await Shell.Current.GoToAsync($"//BookListPage");
                    Debug.WriteLine("Navigated to BookListPage.");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", message, "Tamam");
                    Debug.WriteLine("LoginAsync: Login failed.");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Giriş işlemi sırasında hata: {ex.Message}", "Tamam");
                Debug.WriteLine($"LoginAsync exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            Debug.WriteLine("NavigateToRegister started.");
            try
            {
                if (Shell.Current == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Shell navigasyonu başlatılamadı.", "Tamam");
                    Debug.WriteLine("NavigateToRegister: Shell.Current is null.");
                    return;
                }
                await Shell.Current.GoToAsync("RegisterPage");
                Debug.WriteLine("Navigated to RegisterPage.");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Navigasyon hatası: {ex.Message}", "Tamam");
                Debug.WriteLine($"NavigateToRegister exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}