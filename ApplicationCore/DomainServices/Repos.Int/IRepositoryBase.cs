namespace ApplicationCore.DomainServices.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T?>? GetById(int id);
        Task<List<T>> GetAll();

        Task AddAsync(T t);

        Task Update(T t);

        Task Delete(T t);
    }
}
