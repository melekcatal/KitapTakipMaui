using KitapTakipMaui.ViewModels;
using KitapTakipMaui.Services.Interfaces;

namespace KitapTakipMaui.Views
{
    public partial class AddBookPage : ContentPage
    {
        public AddBookPage()
        {
            InitializeComponent();
            BindingContext = new AddBookViewModel(App.Services.GetService<IBookService>());
        }
    }
}