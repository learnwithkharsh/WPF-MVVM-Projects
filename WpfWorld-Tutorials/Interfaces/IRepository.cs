namespace WpfApp.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task AddBulkAsync(List<T> employees);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<(List<T> employee, int totalEmployee, int skip)> GetPagedAsync(int page, int pageSize);
    }
}
