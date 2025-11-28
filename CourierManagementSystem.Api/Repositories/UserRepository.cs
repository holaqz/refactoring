using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
        
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<List<User>> GetByRoleAsync(UserRole role)
    {
        return await _context.Users
            .Where(u => u.Role == role)
            .ToListAsync();
    }

    public async Task<List<User>> GetAllCouriersAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.courier)
            .ToListAsync();
    }

    public async Task<bool> ExistsByLoginAsync(string login)
    {
        return await _context.Users
            .AnyAsync(u => u.Login == login);
    }
}
