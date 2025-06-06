using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Diagnostics;

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
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            LoadBooksCommand.Execute(null); // Kitapları yükle
            Debug.WriteLine("BookListViewModel initialized.");
        }

        [RelayCommand]
        private async Task LoadBooks()
        {
            Debug.WriteLine($"LoadBooks started with GenreFilter: {GenreFilter}, AuthorFilter: {AuthorFilter}");
            try
            {
                var bookList = await _bookService.GetBooksAsync(GenreFilter, AuthorFilter);
                Books = new ObservableCollection<BookDto>(bookList ?? new List<BookDto>());
                Debug.WriteLine($"LoadBooks completed with {Books.Count} books loaded.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadBooks exception: {ex.Message}\n{ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert("Hata", "Kitaplar yüklenirken bir hata oluştu.", "Tamam");
            }
        }

        [RelayCommand]
        private async Task NavigateToAddBook()
        {
            Debug.WriteLine("NavigateToAddBook started.");
            try
            {
                if (Shell.Current == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Shell navigasyonu başlatılamadı.", "Tamam");
                    Debug.WriteLine("NavigateToAddBook: Shell.Current is null.");
                    return;
                }
                await Shell.Current.GoToAsync("AddBookPage"); // Mutlak rota (//) kaldırıldı
                Debug.WriteLine("Navigated to AddBookPage.");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Navigasyon hatası: {ex.Message}", "Tamam");
                Debug.WriteLine($"NavigateToAddBook exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        [RelayCommand]
        private async Task NavigateToBookDetail(BookDto book)
        {
            Debug.WriteLine($"NavigateToBookDetail started for book ID: {book?.Id}");
            try
            {
                if (Shell.Current == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Shell navigasyonu başlatılamadı.", "Tamam");
                    Debug.WriteLine("NavigateToBookDetail: Shell.Current is null.");
                    return;
                }
                if (book == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Hata", "Kitap bilgisi eksik.", "Tamam");
                    Debug.WriteLine("NavigateToBookDetail: Book is null.");
                    return;
                }
                var parameters = new Dictionary<string, object> { { "BookId", book.Id } };
                await Shell.Current.GoToAsync("BookDetailPage", parameters); // Mutlak rota (//) kaldırıldı
                Debug.WriteLine($"Navigated to BookDetailPage with BookId: {book.Id}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hata", $"Navigasyon hatası: {ex.Message}", "Tamam");
                Debug.WriteLine($"NavigateToBookDetail exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}