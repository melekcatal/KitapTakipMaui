using KitapTakipMaui.Models;
using KitapTakipMaui.Services.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KitapTakipMaui.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private string _token = null;
        private const string BaseUrl = "https://localhost:7220";

        public AuthService(HttpClient httpClient)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = httpClient ?? new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Debug.WriteLine("AuthService initialized with BaseAddress: " + BaseUrl);
        }

        public async Task<(bool Success, string Message, string Token)> LoginAsync(LoginModel model)
        {
            Debug.WriteLine("LoginAsync started.");
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                Debug.WriteLine("LoginAsync: Model is null or fields are empty.");
                return (false, "Kullanıcı adı ve şifre gerekli.", null);
            }

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                Debug.WriteLine($"Sending POST to /api/auth/login with model: {JsonSerializer.Serialize(model)}");
                var response = await _httpClient.PostAsync("/api/auth/login", content);
                Debug.WriteLine($"LoginAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"LoginAsync response content length: {result?.Length ?? 0}, Content: {result ?? "null"}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"LoginAsync failed with status: {response.StatusCode}");
                    return (false, $"Sunucu hatası: {response.StatusCode} - {response.ReasonPhrase}", null);
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    Debug.WriteLine("LoginAsync: Response is empty or whitespace.");
                    return (false, "API yanıtı boş.", null);
                }

                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(result, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse.Success)
                    {
                        _token = apiResponse.Data ?? throw new InvalidOperationException("Token not found in response.");
                        Debug.WriteLine($"LoginAsync: Token retrieved successfully: {_token.Substring(0, Math.Min(10, _token.Length))}...");
                        return (true, apiResponse.Message ?? "Giriş başarılı.", _token);
                    }
                    else
                    {
                        Debug.WriteLine($"LoginAsync failed. Message: {apiResponse?.Message}");
                        return (false, apiResponse?.Message ?? "Giriş başarısız.", null);
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"LoginAsync JsonException: {ex.Message}\nRaw Response: {result}\n{ex.StackTrace}");
                    return (false, $"Yanıt JSON formatında değil: {result}", null);
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"LoginAsync HttpRequestException: {ex.Message}\n{ex.StackTrace}");
                return (false, $"Bağlantı hatası: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoginAsync exception: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\n{ex.StackTrace}");
                return (false, $"Hata: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterModel model)
        {
            Debug.WriteLine("RegisterAsync started.");
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                Debug.WriteLine("RegisterAsync: Model is null or fields are empty.");
                return (false, "Tüm alanlar doldurulmalı.");
            }

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                Debug.WriteLine($"Sending POST to /api/auth/register with model: {JsonSerializer.Serialize(model)}");
                var response = await _httpClient.PostAsync("/api/auth/register", content);
                Debug.WriteLine($"RegisterAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"RegisterAsync response content length: {result?.Length ?? 0}, Content: {result ?? "null"}");

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"RegisterAsync failed with status: {response.StatusCode}");
                    return (false, $"Sunucu hatası: {response.StatusCode} - {response.ReasonPhrase}");
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    Debug.WriteLine("RegisterAsync: Response is empty or whitespace.");
                    return (false, "API yanıtı boş.");
                }

                try
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(result, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse.Success)
                    {
                        Debug.WriteLine("RegisterAsync: Registration successful.");
                        return (true, apiResponse.Message ?? "Kayıt başarılı.");
                    }
                    else
                    {
                        Debug.WriteLine($"RegisterAsync failed. Message: {apiResponse?.Message}");
                        return (false, apiResponse?.Message ?? "Kayıt başarısız.");
                    }
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"RegisterAsync JsonException: {ex.Message}\nRaw Response: {result}\n{ex.StackTrace}");
                    return (false, $"Yanıt JSON formatında değil: {result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"RegisterAsync HttpRequestException: {ex.Message}\n{ex.StackTrace}");
                return (false, $"Bağlantı hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegisterAsync exception: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\n{ex.StackTrace}");
                return (false, $"Hata: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            _token = null;
            Debug.WriteLine("LogoutAsync completed.");
        }

        public string GetToken()
        {
            Debug.WriteLine($"GetToken: Returning token: {_token?.Substring(0, Math.Min(10, _token?.Length ?? 0))}...");
            return _token;
        }
    }
}