using CarService.Entities.Entities;

namespace CarService.Business.Abstract
{
    public interface IShopService
    {
        Task<IEnumerable<Shop>> GetAllShopsAsync();
        Task<Shop> GetShopByIdAsync(Guid id);
        Task<Shop> CreateShopAsync(Shop shopDto);
        Task<Shop?> GetShopByUserIdAsync(Guid userId);
        Task UpdateShopAsync(Guid id, Shop shopDto);
        Task DeleteShopAsync(Guid id);
        Task<IEnumerable<Shop>> GetShopsByCategoryAsync(string category);
        Task<IEnumerable<Shop>> SearchShopsAsync(string query);
    }
}
