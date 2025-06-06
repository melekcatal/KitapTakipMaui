using KitapTakipMaui.ViewModels;
using KitapTakipMaui.Services.Interfaces;

namespace KitapTakipMaui.Views
{
    [QueryProperty(nameof(BookId), "BookId")]
    public partial class EditBookPage : ContentPage
    {
        private int _bookId;
        public int BookId
        {
            get => _bookId;
            set
            {
                _bookId = value;
                BindingContext = new EditBookViewModel(App.Services.GetService<IBookService>(), _bookId);
            }
        }

        public EditBookPage()
        {
            InitializeComponent();
        }
    }
}