using ApiExamen.Models;

namespace ApiExamen.Interfaces
{
    public interface IAuthService
    {
        Task<string?> Login(LoginRequest request);
    }
} 