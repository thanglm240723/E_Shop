using EcommerceAPI.Core.Interfaces.Repositories;
using EcommerceAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceAPI.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
           await _dbSet.AddAsync(entity);
           await _context.SaveChangesAsync();
           return entity;
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
           if(predicate == null)
           {
               return await _dbSet.CountAsync();
           }
           else
           {
               return await _dbSet.CountAsync(predicate);
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
             _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

        }

        public virtual async  Task<bool> ExistsAsync(int id)
        {
            var entity = _dbSet.Find(id);
            //return Task.FromResult(entity != null);
            // return entity != null ? Task.FromResult(true) : Task.FromResult(false);
            return entity != null;
        }

        public virtual async  Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
           return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
         return  await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
           return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();


        }
    }
}
