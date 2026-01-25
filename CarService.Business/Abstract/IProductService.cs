using CarService.Entities.Entities;

namespace CarService.Business.Abstract
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid id);
        Task<IEnumerable<Product>> GetProductsByShopIdAsync(Guid shopId);
    }
}
