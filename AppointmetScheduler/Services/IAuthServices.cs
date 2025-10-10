using AppointmetScheduler.Entities;
using AppointmetScheduler.Models;

namespace AppointmetScheduler.Services
{
    public interface IAuthServices
    {
        Task<User?> RegisterAsync(RegDto request);
        Task<string?> LoginAsync(UserDto request);
        //Task<RefreshTokenDto> RefreshTokenAsync(RefreshTokenDto request);
    }
}
