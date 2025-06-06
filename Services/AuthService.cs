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
        private const string BaseUrl = "https://localhost:7220/api/auth";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(BaseUrl); // Base adresi ayarla
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
                Debug.WriteLine($"Sending POST to /login with model: {JsonSerializer.Serialize(model)}");
                var response = await _httpClient.PostAsync("/login", content);
                Debug.WriteLine($"LoginAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"LoginAsync response content: {result}");
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(result);

                if (response.IsSuccessStatusCode && apiResponse.Success)
                {
                    _token = apiResponse.Data ?? throw new InvalidOperationException("Token not found in response.");
                    Debug.WriteLine($"LoginAsync: Token retrieved successfully: {_token}");
                    return (true, apiResponse.Message ?? "Giriş başarılı.", _token);
                }
                else
                {
                    Debug.WriteLine($"LoginAsync failed. Status: {response.StatusCode}, Message: {apiResponse?.Message}");
                    return (false, apiResponse?.Message ?? "Giriş başarısız.", null);
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"LoginAsync HttpRequestException: {ex.Message}\n{ex.StackTrace}");
                return (false, $"Bağlantı hatası: {ex.Message}", null);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"LoginAsync JsonException: {ex.Message}\n{ex.StackTrace}");
                return (false, "Yanıt JSON formatında değil.", null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoginAsync exception: {ex.Message}\n{ex.StackTrace}");
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
                Debug.WriteLine($"Sending POST to /register with model: {JsonSerializer.Serialize(model)}");
                var response = await _httpClient.PostAsync("/register", content);
                Debug.WriteLine($"RegisterAsync response: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");

                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"RegisterAsync response content: {result}");
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(result);

                if (response.IsSuccessStatusCode && apiResponse.Success)
                {
                    Debug.WriteLine("RegisterAsync: Registration successful.");
                    return (true, apiResponse.Message ?? "Kayıt başarılı.");
                }
                else
                {
                    Debug.WriteLine($"RegisterAsync failed. Status: {response.StatusCode}, Message: {apiResponse?.Message}");
                    return (false, apiResponse?.Message ?? "Kayıt başarısız.");
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"RegisterAsync HttpRequestException: {ex.Message}\n{ex.StackTrace}");
                return (false, $"Bağlantı hatası: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"RegisterAsync JsonException: {ex.Message}\n{ex.StackTrace}");
                return (false, "Yanıt JSON formatında değil.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegisterAsync exception: {ex.Message}\n{ex.StackTrace}");
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
            Debug.WriteLine($"GetToken: Returning token: {_token}");
            return _token;
        }
    }
}