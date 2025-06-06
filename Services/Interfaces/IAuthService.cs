using KitapTakipMaui.Models;
using System.Threading.Tasks;

namespace KitapTakipMaui.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string Token)> LoginAsync(LoginModel model);
        Task<(bool Success, string Message)> RegisterAsync(RegisterModel model);
        Task LogoutAsync();
        string GetToken();
    }
}