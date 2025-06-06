using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace KitapTakipMaui.ViewModels
{
    public partial class AddBookViewModel : ObservableObject
    {
        private readonly IBookService _bookService;

        [ObservableProperty]
        private BookDto book = new();

        public AddBookViewModel(IBookService bookService)
        {
            _bookService = bookService;
            Book.StartDate = DateTime.Today;
        }

        [RelayCommand]
        private async Task AddBook()
        {
            if (string.IsNullOrEmpty(Book.Title) || string.IsNullOrEmpty(Book.Author) || string.IsNullOrEmpty(Book.Genre) || Book.PageCount <= 0)
            {
                await Shell.Current.DisplayAlert("Hata", "Zorunlu alanlar doldurulmalı ve sayfa sayısı sıfırdan büyük olmalı.", "Tamam");
                return;
            }

            var success = await _bookService.AddBookAsync(Book);
            if (success)
            {
                await Shell.Current.GoToAsync($"//BookListPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Hata", "Kitap ekleme başarısız.", "Tamam");
            }
        }
    }
}