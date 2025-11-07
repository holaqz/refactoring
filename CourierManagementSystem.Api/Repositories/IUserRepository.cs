using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByLoginAsync(string login);
    Task<List<User>> GetByRoleAsync(UserRole role);
    Task<List<User>> GetAllCouriersAsync();
    Task<List<User>> GetAllManagersAsync();
    Task<bool> ExistsByLoginAsync(string login);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<int> SaveChangesAsync();
}
