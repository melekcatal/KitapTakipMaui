using KitapTakipMaui.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitapTakipMaui.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<BookDto>> GetBooksAsync(string? genre = null, string? author = null);
        Task<BookDto> GetBookByIdAsync(int id);
        Task<bool> AddBookAsync(BookDto bookDto);
        Task<bool> UpdateBookAsync(int id, BookDto bookDto);
        Task<bool> DeleteBookAsync(int id);
        Task<BookDetailsDto> GetBookDetailsByIdAsync(int id);
    }
}