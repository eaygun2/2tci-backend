namespace ApplicationCore.DomainServices.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        T? GetById(int id);
        List<T> GetAll();

        Task AddAsync(T t);

        Task Update(T t);

        Task Delete(T t);
    }
}
