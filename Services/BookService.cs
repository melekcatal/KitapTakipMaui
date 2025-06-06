using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KitapTakipMaui.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private const string BaseUrl = "https://localhost:7220/api/books";

        public BookService(HttpClient httpClient, IAuthService authService)
        {
            // HTTPS sertifikası hatasını geçici olarak devre dışı bırak (geliştirme ortamı için)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = httpClient ?? new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7220"); // BaseAddress açıkça ayarlandı
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            Debug.WriteLine("BookService initialized with BaseAddress: " + _httpClient.BaseAddress);
        }

        private void AddAuthorizationHeader()
        {
            var token = _authService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Debug.WriteLine($"Authorization header added with token: {token.Substring(0, Math.Min(10, token.Length))}...");
            }
            else
            {
                Debug.WriteLine("No token available for Authorization header.");
            }
        }

        public async Task<List<BookDto>> GetBooksAsync(string? genre = null, string? author = null)
        {
            Debug.WriteLine($"GetBooksAsync started with Genre: {genre}, Author: {author}");
            try
            {
                AddAuthorizationHeader();
                var query = string.Empty;
                if (!string.IsNullOrEmpty(genre) || !string.IsNullOrEmpty(author))
                {
                    query = $"?genre={Uri.EscapeDataString(genre ?? "")}&author={Uri.EscapeDataString(author ?? "")}";
                }
                var response = await _httpClient.GetAsync($"{BaseUrl}{query}");
                Debug.WriteLine($"GetBooksAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"GetBooksAsync response content length: {content?.Length ?? 0}, Content: {content ?? "null"}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"GetBooksAsync failed with status: {response.StatusCode}");
                    return new List<BookDto>();
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    Debug.WriteLine("GetBooksAsync: Response is empty or whitespace.");
                    return new List<BookDto>();
                }

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<BookDto>>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                var result = apiResponse?.Data ?? new List<BookDto>();
                Debug.WriteLine($"GetBooksAsync completed with {result.Count} books.");
                return result;
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"GetBooksAsync HttpRequestException: {ex.Message}\n{ex.StackTrace}");
                return new List<BookDto>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"GetBooksAsync JsonException: {ex.Message}\n{ex.StackTrace}");
                return new List<BookDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetBooksAsync exception: {ex.Message}\n{ex.StackTrace}");
                return new List<BookDto>();
            }
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            Debug.WriteLine($"GetBookByIdAsync started for ID: {id}");
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                Debug.WriteLine($"GetBookByIdAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"GetBookByIdAsync response content: {content}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"GetBookByIdAsync failed with status: {response.StatusCode}");
                    return new BookDto();
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    Debug.WriteLine("GetBookByIdAsync: Response is empty or whitespace.");
                    return new BookDto();
                }

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<BookDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                var result = apiResponse?.Data ?? new BookDto();
                Debug.WriteLine($"GetBookByIdAsync completed for ID: {id}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetBookByIdAsync exception: {ex.Message}\n{ex.StackTrace}");
                return new BookDto();
            }
        }

        public async Task<bool> AddBookAsync(BookDto bookDto)
        {
            Debug.WriteLine($"AddBookAsync started for book: {bookDto?.Title}");
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, bookDto);
                Debug.WriteLine($"AddBookAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddBookAsync exception: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> UpdateBookAsync(int id, BookDto bookDto)
        {
            Debug.WriteLine($"UpdateBookAsync started for ID: {id}");
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", bookDto);
                Debug.WriteLine($"UpdateBookAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateBookAsync exception: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            Debug.WriteLine($"DeleteBookAsync started for ID: {id}");
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                Debug.WriteLine($"DeleteBookAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteBookAsync exception: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public async Task<BookDetailsDto> GetBookDetailsByIdAsync(int id)
        {
            Debug.WriteLine($"GetBookDetailsByIdAsync started for ID: {id}");
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"{BaseUrl}/details/{id}");
                Debug.WriteLine($"GetBookDetailsByIdAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"GetBookDetailsByIdAsync response content: {content}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"GetBookDetailsByIdAsync failed with status: {response.StatusCode}");
                    return new BookDetailsDto();
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    Debug.WriteLine("GetBookDetailsByIdAsync: Response is empty or whitespace.");
                    return new BookDetailsDto();
                }

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<BookDetailsDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                var result = apiResponse?.Data ?? new BookDetailsDto();
                Debug.WriteLine($"GetBookDetailsByIdAsync completed for ID: {id}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetBookDetailsByIdAsync exception: {ex.Message}\n{ex.StackTrace}");
                return new BookDetailsDto();
            }
        }
    }
}