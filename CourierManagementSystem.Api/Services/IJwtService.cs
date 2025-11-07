using CourierManagementSystem.Api.Models.Entities;
using System.Security.Claims;

namespace CourierManagementSystem.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
    long? GetUserIdFromToken(string token);
}
