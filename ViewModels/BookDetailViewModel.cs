using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System.Threading.Tasks;

namespace KitapTakipMaui.ViewModels
{
    public partial class BookDetailViewModel : ObservableObject
    {
        private readonly IBookService _bookService;
        private readonly int _bookId;

        [ObservableProperty]
        private BookDetailsDto bookDetails = new();

        public BookDetailViewModel(IBookService bookService, int bookId)
        {
            _bookService = bookService;
            _bookId = bookId;
            LoadBookDetailsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadBookDetails()
        {
            BookDetails = await _bookService.GetBookDetailsByIdAsync(_bookId);
        }

        [RelayCommand]
        private async Task NavigateToEditBook()
        {
            var parameters = new Dictionary<string, object> { { "BookId", _bookId } };
            await Shell.Current.GoToAsync($"//EditBookPage", parameters);
        }

        [RelayCommand]
        private async Task DeleteBook()
        {
            var confirm = await Shell.Current.DisplayAlert("Sil", "Kitabı silmek istediğinize emin misiniz?", "Evet", "Hayır");
            if (confirm)
            {
                var success = await _bookService.DeleteBookAsync(_bookId);
                if (success)
                {
                    await Shell.Current.GoToAsync($"//BookListPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hata", "Silme başarısız.", "Tamam");
                }
            }
        }
    }
}