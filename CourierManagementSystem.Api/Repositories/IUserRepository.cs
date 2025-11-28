using CourierManagementSystem.Api.Models.Entities;

namespace CourierManagementSystem.Api.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByLoginAsync(string login);
    Task<List<User>> GetByRoleAsync(UserRole role);
    Task<List<User>> GetAllCouriersAsync();
    Task<bool> ExistsByLoginAsync(string login);

}
