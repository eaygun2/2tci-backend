using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class CarInspectionsDbContext : DbContext
    {
        public CarInspectionsDbContext(DbContextOptions<CarInspectionsDbContext> options) : base(options) { }
        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
