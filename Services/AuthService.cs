using HotelApp.Api.Data;
using HotelApp.Api.DTOs;
using HotelApp.Api.Helpers;
using HotelApp.Api.Interfaces;
using HotelApp.Api.Models;
using MongoDB.Driver;

namespace HotelApp.Api.Services;

public sealed class AuthService : IAuthService
{
    private readonly MongoDbService _mongoDbService;
    private readonly JwtHelper _jwtHelper;

    public AuthService(MongoDbService mongoDbService, JwtHelper jwtHelper)
    {
        _mongoDbService = mongoDbService;
        _jwtHelper = jwtHelper;
    }

    public async Task<User> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _mongoDbService.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = dto.Role
        };

        await _mongoDbService.Users.InsertOneAsync(user);
        return user;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var user = await _mongoDbService.Users.Find(u => u.Email == dto.Email).FirstOrDefaultAsync();
        if (user is null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
        {
            return null;
        }

        return _jwtHelper.GenerateToken(user);
    }
}
