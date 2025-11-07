using CourierManagementSystem.Api.Data;
using CourierManagementSystem.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierManagementSystem.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users.FindAsync(id);
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

    public async Task<List<User>> GetAllManagersAsync()
    {
        return await _context.Users
            .Where(u => u.Role == UserRole.manager)
            .ToListAsync();
    }

    public async Task<bool> ExistsByLoginAsync(string login)
    {
        return await _context.Users
            .AnyAsync(u => u.Login == login);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return user;
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
