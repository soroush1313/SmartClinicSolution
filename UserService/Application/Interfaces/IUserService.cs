using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IUserService
    {
        Task<Guid> RegisterAsync(RegisterRequest request);
        Task<string> LoginAsync(string email, string password);
    }
}
