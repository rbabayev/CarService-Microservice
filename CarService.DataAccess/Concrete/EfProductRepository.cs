using CarService.DataAccess.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.DataAccess.Concrete
{
    public class EfProductRepository : IProductRepository
    {
        private readonly CarServiceDbContext _context;

        public EfProductRepository(CarServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existingProduct = await _context.Products.AsNoTracking()
                                                         .FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException("Product not found.");

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByShopIdAsync(Guid shopId)
        {
            return await _context.Products
                                 .Where(p => p.ShopId == shopId)
                                 .ToListAsync();
        }
    }
}
