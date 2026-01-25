using CarService.Business.Abstract;
using CarService.DataAccess.Abstract;
using CarService.Entities.Entities;

namespace CarService.Business.Concrete
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;

        public ShopService(IShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
        }

        public async Task<IEnumerable<Shop>> GetAllShopsAsync()
        {
            return await _shopRepository.GetAllAsync();
        }

        public async Task<Shop> GetShopByIdAsync(Guid id)
        {
            return await _shopRepository.GetByIdAsync(id) ?? throw new Exception("Shop not found");
        }

        public async Task<Shop> GetShopByUserIdAsync(Guid userId)
        {
            return await _shopRepository.GetByUserIdAsync(userId) ?? throw new Exception("User not found");
        }

        public async Task<Shop> CreateShopAsync(Shop shopDto)
        {
            var shop = new Shop
            {
                Id = Guid.NewGuid(),
                ShopName = shopDto.ShopName,
                ShopCategory = shopDto.ShopCategory,
                ShopImageUrl = shopDto.ShopImageUrl,
                UserId = shopDto.UserId,
                // Diğer alanlar burada ayarlanabilir.
            };

            await _shopRepository.AddAsync(shop);
            return shop;
        }

        public async Task UpdateShopAsync(Guid id, Shop shopDto)
        {
            var shop = await _shopRepository.GetByIdAsync(id);
            if (shop == null)
                throw new Exception("Shop not found");

            shop.ShopName = shopDto.ShopName;
            shop.ShopCategory = shopDto.ShopCategory;
            // Diğer alanlar burada güncellenebilir.

            await _shopRepository.UpdateAsync(shop);
        }

        public async Task DeleteShopAsync(Guid id)
        {
            var shop = await _shopRepository.GetByIdAsync(id);
            if (shop == null)
                throw new Exception("Shop not found");

            await _shopRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<Shop>> GetShopsByCategoryAsync(string category)
        {
            var shops = await _shopRepository.GetAllAsync();
            return shops.Where(s => s.ShopCategory.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Shop>> SearchShopsAsync(string query)
        {
            var shops = await _shopRepository.GetAllAsync();
            return shops.Where(s => s.ShopName.Contains(query, StringComparison.OrdinalIgnoreCase));
        }
    }
}
