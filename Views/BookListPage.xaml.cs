using KitapTakipMaui.ViewModels;
using KitapTakipMaui.Services.Interfaces;

namespace KitapTakipMaui.Views
{
    public partial class BookListPage : ContentPage
    {
        public BookListPage()
        {
            InitializeComponent();
            BindingContext = new BookListViewModel(App.Services.GetService<IBookService>());
        }
    }
}