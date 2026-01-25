using CarService.Entities.Entities;

namespace CarService.Business.Abstract
{
    public interface IWorkerService
    {
        Task<IEnumerable<Worker>> GetAllWorkersAsync();
        Task<Worker> GetWorkerByIdAsync(Guid id);
        Task<Worker> CreateWorkerAsync(Worker workerDto);
        Task UpdateWorkerAsync(Guid id, Worker workerDto);
        Task DeleteWorkerAsync(Guid id);
        Task<IEnumerable<Worker>> SearchWorkersAsync(string query);
        Task<IEnumerable<Worker>> GetWorkersByCategoryAsync(string category);
    }
}
