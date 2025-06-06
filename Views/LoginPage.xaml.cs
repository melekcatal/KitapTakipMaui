using KitapTakipMaui.ViewModels;
using KitapTakipMaui.Services.Interfaces;
using System.Diagnostics;

namespace KitapTakipMaui.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            if (App.Services == null)
            {
                Debug.WriteLine("LoginPage: App.Services is null.");
                throw new InvalidOperationException("App.Services is null.");
            }
            var authService = App.Services.GetService<IAuthService>();
            if (authService == null)
            {
                Debug.WriteLine("LoginPage: IAuthService could not be resolved.");
                throw new InvalidOperationException("IAuthService could not be resolved.");
            }
            BindingContext = new LoginViewModel(authService);
            Debug.WriteLine("LoginPage initialized.");
        }
    }
}