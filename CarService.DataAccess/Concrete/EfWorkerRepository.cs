using CarService.DataAccess.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.DataAccess.Concrete
{
    public class EfWorkerRepository : IWorkerRepository
    {
        private readonly CarServiceDbContext _context;

        public EfWorkerRepository(CarServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Worker worker)
        {
            await _context.Set<Worker>().AddAsync(worker);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var worker = await GetByIdAsync(id);
            if (worker != null)
            {
                _context.Set<Worker>().Remove(worker);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Worker>> GetAllAsync()
        {
            return await _context.Set<Worker>().ToListAsync();
        }

        public async Task<Worker> GetByIdAsync(Guid id)
        {
            return await _context.Set<Worker>().FindAsync(id);
        }

        public async Task<IEnumerable<Worker>> SearchByNameAsync(string query)
        {
            return await _context.Set<Worker>()
                 .Where(s => s.FullName.Contains(query, StringComparison.OrdinalIgnoreCase))
                 .ToListAsync();
        }

        public async Task UpdateAsync(Worker worker)
        {
            _context.Set<Worker>().Update(worker);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Worker>> GetByCategoryAsync(string category)
        {
            return await _context.Set<Worker>().Where(s => s.WorkerCategory.Equals(category, StringComparison.OrdinalIgnoreCase)).ToListAsync();
        }
    }
}
