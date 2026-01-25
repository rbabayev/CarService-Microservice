using CarService.Business.Abstract;
using CarService.DataAccess.Abstract;
using CarService.Entities.Entities;

namespace CarService.Business.Concrete
{
    public class WorkerService : IWorkerService
    {
        private readonly IWorkerRepository _workerRepository;

        public WorkerService(IWorkerRepository workerRepository)
        {
            _workerRepository = workerRepository;
        }

        public async Task<Worker> CreateWorkerAsync(Worker workerDto)
        {
            var worker = new Worker
            {
                Id = Guid.NewGuid(),
                FullName = workerDto.FullName,
                WorkerCategory = workerDto.WorkerCategory,
                ProfileImageUrl = workerDto.ProfileImageUrl,
                UserId = workerDto.UserId,
                // Diğer alanlar burada ayarlanabilir.
            };

            await _workerRepository.AddAsync(worker);
            return worker;
        }

        public async Task DeleteWorkerAsync(Guid id)
        {
            var worker = await _workerRepository.GetByIdAsync(id);
            if (worker == null)
                throw new Exception("worker not found");

            await _workerRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Worker>> GetAllWorkersAsync()
        {
            return await _workerRepository.GetAllAsync();
        }

        public async Task<Worker> GetWorkerByIdAsync(Guid id)
        {
            return await _workerRepository.GetByIdAsync(id) ?? throw new Exception("Worker not found");

        }

        public async Task<IEnumerable<Worker>> GetWorkersByCategoryAsync(string category)
        {
            var workers = await _workerRepository.GetAllAsync();
            return workers.Where(s => s.WorkerCategory.Equals(category, StringComparison.OrdinalIgnoreCase));

        }

        public async Task<IEnumerable<Worker>> SearchWorkersAsync(string query)
        {
            var workers = await _workerRepository.GetAllAsync();
            return workers.Where(s => s.FullName.Contains(query, StringComparison.OrdinalIgnoreCase));

        }

        public async Task UpdateWorkerAsync(Guid id, Worker workerDto)
        {
            var worker = await _workerRepository.GetByIdAsync(id);
            if (worker == null)
                throw new Exception("Shop not found");

            worker.FullName = workerDto.FullName;
            worker.WorkerCategory = workerDto.WorkerCategory;

            await _workerRepository.UpdateAsync(worker);
        }
    }
}
