using ApplicationCore.DomainServices.Interfaces;
using ApplicationCore.Entities;
using Infrastructure.Data;
using System.Data.Entity;

namespace Infrastructure.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase, new()
    {
        private readonly CarInspectionsDbContext _context;

        public RepositoryBase(CarInspectionsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(CarInspectionsDbContext));
        }

        public async Task AddAsync(TEntity? t)
        {
           if(t == null) throw new ArgumentNullException("Entity given cannot be null");

           await _context.Set<TEntity>().AddAsync(t);
           await _context.SaveChangesAsync();
        }

        public async Task Delete(TEntity t)
        {
            if (t == null || await GetById(t.Id) == null) throw new NullReferenceException("Given entity cannot be null or does not exist");

            _context.Set<TEntity>().Remove(t);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?>? GetById(int id)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(TEntity t)
        {
            if (t == null || await GetById(t.Id) == null) throw new NullReferenceException("Given entity cannot be null or does not exist");

            _context.Set<TEntity>().Update(t);
            await _context.SaveChangesAsync();
        }
    }
}
