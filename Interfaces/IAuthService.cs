using HotelApp.Api.DTOs;
using HotelApp.Api.Models;

namespace HotelApp.Api.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
}
