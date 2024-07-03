using ApplicationCore.Entities;
using ApplicationCore.Entities.DataTransferObjects;
using ApplicationCore.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class CarInspectionsDbContext : DbContext
    {
        public DbSet<DetectionModelOutputDto> CarObjectDetectionPredictions { get; set; }


        public CarInspectionsDbContext(DbContextOptions<CarInspectionsDbContext> options) : base(options) { }

        public DbSet<TEntity> Set<TEntity>() where TEntity : EntityBase, IModelOutput
        {
            return base.Set<TEntity>();
        }
    }
}
