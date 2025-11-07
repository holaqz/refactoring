using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Models.DTOs;

public class UserDto
{
    public long Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UserDto From(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Name = user.Name,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
