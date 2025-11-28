// Repository.cs
using CourierManagementSystem.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourierManagementSystem.Api.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> 
    where TEntity : class
{
    protected readonly ApplicationDbContext _context;

    protected Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(long id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        return entity;
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}