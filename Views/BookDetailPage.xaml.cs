using KitapTakipMaui.ViewModels;
using KitapTakipMaui.Services.Interfaces;

namespace KitapTakipMaui.Views
{
    [QueryProperty(nameof(BookId), "BookId")]
    public partial class BookDetailPage : ContentPage
    {
        private int _bookId;
        public int BookId
        {
            get => _bookId;
            set
            {
                _bookId = value;
                BindingContext = new BookDetailViewModel(App.Services.GetService<IBookService>(), _bookId);
            }
        }

        public BookDetailPage()
        {
            InitializeComponent();
        }
    }
}