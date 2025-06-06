using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System.Threading.Tasks;

namespace KitapTakipMaui.ViewModels
{
    public partial class EditBookViewModel : ObservableObject
    {
        private readonly IBookService _bookService;
        private readonly int _bookId;

        [ObservableProperty]
        private BookDto book = new();

        public EditBookViewModel(IBookService bookService, int bookId)
        {
            _bookService = bookService;
            _bookId = bookId;
            LoadBookCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadBook()
        {
            Book = await _bookService.GetBookByIdAsync(_bookId);
        }

        [RelayCommand]
        private async Task UpdateBook()
        {
            if (string.IsNullOrEmpty(Book.Title) || string.IsNullOrEmpty(Book.Author) || string.IsNullOrEmpty(Book.Genre) || Book.PageCount <= 0)
            {
                await Shell.Current.DisplayAlert("Hata", "Zorunlu alanlar doldurulmalı ve sayfa sayısı sıfırdan büyük olmalı.", "Tamam");
                return;
            }

            var success = await _bookService.UpdateBookAsync(_bookId, Book);
            if (success)
            {
                await Shell.Current.GoToAsync($"//BookListPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", "Güncelleme başarısız.", "Tamam");
            }
        }
    }
}