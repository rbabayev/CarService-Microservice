using CarService.Entities.Entities;

namespace CarService.DataAccess.Abstract
{
    public interface IWorkerRepository
    {
        Task<IEnumerable<Worker>> GetAllAsync();
        Task<Worker> GetByIdAsync(Guid id);
        Task AddAsync(Worker worker);
        Task UpdateAsync(Worker worker);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Worker>> GetByCategoryAsync(string category);
        Task<IEnumerable<Worker>> SearchByNameAsync(string query);
    }
}
