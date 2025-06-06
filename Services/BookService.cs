using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace KitapTakipMaui.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private const string BaseUrl = "https://localhost:7220/api/books";

        public BookService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private void AddAuthorizationHeader()
        {
            var token = _authService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<BookDto>> GetBooksAsync(string? genre = null, string? author = null)
        {
            AddAuthorizationHeader();
            var query = string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(author) ? "" : $"?genre={genre}&author={author}";
            var response = await _httpClient.GetAsync($"{BaseUrl}{query}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<BookDto>>>();
                return apiResponse?.Data ?? new List<BookDto>();
            }
            return new List<BookDto>();
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>();
                return apiResponse?.Data ?? new BookDto();
            }
            return new BookDto();
        }

        public async Task<bool> AddBookAsync(BookDto bookDto)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, bookDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBookAsync(int id, BookDto bookDto)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", bookDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<BookDetailsDto> GetBookDetailsByIdAsync(int id)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/details/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDetailsDto>>();
                return apiResponse?.Data ?? new BookDetailsDto();
            }
            return new BookDetailsDto();
        }
    }
}