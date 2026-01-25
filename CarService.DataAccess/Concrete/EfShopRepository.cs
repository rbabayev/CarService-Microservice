using CarService.DataAccess.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.DataAccess.Concrete
{
    public class EfShopRepository : IShopRepository
    {
        private readonly CarServiceDbContext _context;

        public EfShopRepository(CarServiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Shop>> GetAllAsync()
        {
            return await _context.Set<Shop>().ToListAsync();
        }

        public async Task<Shop> GetByIdAsync(Guid id)
        {
            return await _context.Set<Shop>().FindAsync(id);
        }

        public async Task<Shop?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Set<Shop>()
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }


        public async Task AddAsync(Shop shop)
        {
            await _context.Set<Shop>().AddAsync(shop);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shop shop)
        {
            _context.Set<Shop>().Update(shop);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var shop = await GetByIdAsync(id);
            if (shop != null)
            {
                _context.Set<Shop>().Remove(shop);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Shop>> GetByCategoryAsync(string category)
        {
            return await _context.Set<Shop>()
                .Where(s => s.ShopCategory.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<IEnumerable<Shop>> SearchByNameAsync(string query)
        {
            return await _context.Set<Shop>()
                .Where(s => s.ShopName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

    }
}
