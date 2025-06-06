using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KitapTakipMaui.ViewModels
{
    public partial class BookListViewModel : ObservableObject
    {
        private readonly IBookService _bookService;

        [ObservableProperty]
        private ObservableCollection<BookDto> books = new();

        [ObservableProperty]
        private string genreFilter = string.Empty;

        [ObservableProperty]
        private string authorFilter = string.Empty;

        public BookListViewModel(IBookService bookService)
        {
            _bookService = bookService;
            LoadBooksCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadBooks()
        {
            var bookList = await _bookService.GetBooksAsync(GenreFilter, AuthorFilter);
            Books = new ObservableCollection<BookDto>(bookList);
        }

        [RelayCommand]
        private async Task NavigateToAddBook()
        {
            await Shell.Current.GoToAsync($"//AddBookPage");
        }

        [RelayCommand]
        private async Task NavigateToBookDetail(BookDto book)
        {
            var parameters = new Dictionary<string, object> { { "BookId", book.Id } };
            await Shell.Current.GoToAsync($"//BookDetailPage", parameters);
        }
    }
}