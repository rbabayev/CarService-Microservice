using CarService.Business.Abstract;
using CarService.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopsController(IShopService shopService)
        {
            _shopService = shopService;
        }

        // GET: api/shops
        [HttpGet("GetAllShops")]
        public async Task<IActionResult> GetAllShops()
        {
            var shops = await _shopService.GetAllShopsAsync();
            return Ok(shops);
        }


        [HttpGet("GetShopByUser/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetShopByUser(Guid userId)
        {
            try
            {
                var shop = await _shopService.GetShopByUserIdAsync(userId);
                if (shop == null)
                {
                    return NotFound(new { message = "Shop not found for this user" });
                }
                return Ok(shop);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/shops/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopById(Guid id)
        {
            var shop = await _shopService.GetShopByIdAsync(id);
            if (shop == null)
                return NotFound(new { Message = "Shop not found" });

            return Ok(shop);
        }

        // POST: api/shops
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] Shop shop)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdShop = await _shopService.CreateShopAsync(shop);
            return CreatedAtAction(nameof(GetShopById), new { id = createdShop.Id }, createdShop);
        }

        // PUT: api/shops/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShop(Guid id, [FromBody] Shop shop)
        {
            if (id != shop.Id)
                return BadRequest(new { Message = "ID mismatch" });

            var existingShop = await _shopService.GetShopByIdAsync(id);
            if (existingShop == null)
                return NotFound(new { Message = "Shop not found" });

            await _shopService.UpdateShopAsync(id, shop);
            return NoContent();
        }

        // DELETE: api/shops/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(Guid id)
        {
            var shop = await _shopService.GetShopByIdAsync(id);
            if (shop == null)
                return NotFound(new { Message = "Shop not found" });

            await _shopService.DeleteShopAsync(id);
            return NoContent();
        }

        // Get Shops by Category
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetShopsByCategory(string category)
        {
            var shops = await _shopService.GetShopsByCategoryAsync(category);
            if (shops == null || !shops.Any())
                return NotFound(new { Message = "No shops found in this category" });

            return Ok(shops);
        }

        // Search Shops by Name
        [HttpGet("search")]
        public async Task<IActionResult> SearchShops([FromQuery] string query)
        {
            var shops = await _shopService.SearchShopsAsync(query);
            if (shops == null || !shops.Any())
                return NotFound(new { Message = "No shops found matching your query" });

            return Ok(shops);
        }
    }
}
