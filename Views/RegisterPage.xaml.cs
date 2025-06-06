using KitapTakipMaui.ViewModels;
using KitapTakipMaui.Services.Interfaces;

namespace KitapTakipMaui.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel(App.Services.GetService<IAuthService>());
        }
    }
}