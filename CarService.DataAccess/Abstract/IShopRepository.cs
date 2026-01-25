using CarService.Entities.Entities;

namespace CarService.DataAccess.Abstract
{
    public interface IShopRepository
    {
        Task<IEnumerable<Shop>> GetAllAsync();
        Task<Shop> GetByUserIdAsync(Guid userId);
        Task<Shop> GetByIdAsync(Guid id);
        Task AddAsync(Shop shop);
        Task UpdateAsync(Shop shop);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Shop>> GetByCategoryAsync(string category);
        Task<IEnumerable<Shop>> SearchByNameAsync(string query);

    }
}
