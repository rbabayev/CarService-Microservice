using AutoMapper;
using CarService.Business.Abstract;
using CarService.Entities.Entities;
using CarServiceBG.DTOs;
using CarServiceBG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IShopService _shopService;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ProductsController(
            IProductService productService,
            IShopService shopService,
            IPhotoService photoService,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _productService = productService;
            _photoService = photoService;
            _mapper = mapper;
            _shopService = shopService;
            _userManager = userManager;
        }


        [Authorize(Roles = "Seller")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid form data." });
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                return Unauthorized(new { message = "JWT token içinde User ID bulunamadı." });
            }

            if (!Guid.TryParse(userIdClaim, out Guid parsedUserId))
            {
                return Unauthorized(new
                {
                    message = "JWT içindeki User ID geçerli bir GUID formatında değil.",
                    rawUserId = userIdClaim
                });
            }


            var shop = await _shopService.GetShopByUserIdAsync(parsedUserId);
            if (shop == null)
            {
                return BadRequest(new { success = false, message = "You do not have a registered shop." });
            }

            string? imageUrl = null;
            if (productDto.Photo != null)
            {
                var photoDto = new PhotoCreateDto { File = productDto.Photo };
                imageUrl = await _photoService.UploadImageAsync(photoDto);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return BadRequest(new { success = false, message = "Photo upload failed." });
                }
            }

            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                UserId = parsedUserId.ToString(),
                Name = productDto.Name,
                ShopId = shop.Id,
                StockQuantity = productDto.StockQuantity,
                Price = productDto.Price,
                ProductImage = imageUrl
            };

            await _productService.AddProductAsync(newProduct);

            return Ok(new
            {
                success = true,
                message = "Product added successfully.",
                product = newProduct
            });
        }



        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductDto productDto)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            // Yeni foto varsa yüklə və url dəyişdir
            if (productDto.Photo is not null)
            {
                var photoDto = new PhotoCreateDto { File = productDto.Photo };
                var imageUrl = await _photoService.UploadImageAsync(photoDto);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    product.ProductImage = imageUrl;
                }
            }

            // ID-ni qoruyaraq digər sahələri yenilə
            product.Name = productDto.Name ?? product.Name;
            product.StockQuantity = productDto.StockQuantity ?? product.StockQuantity;
            product.Price = productDto.Price ?? product.Price;

            await _productService.UpdateProductAsync(product);

            return Ok(new { Message = "Product updated successfully.", Product = product });
        }



        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            await _productService.DeleteProductAsync(id);

            return Ok(new { Message = "Product deleted successfully." });
        }


        [HttpGet("GetAllProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(productDtos);
        }


        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            var productDto = _mapper.Map<ProductDto>(product);

            return Ok(productDto);
        }

        [HttpGet("GetProductsByShop/{shopId}")]
        public async Task<IActionResult> GetProductsByShop(Guid shopId)
        {
            var products = await _productService.GetProductsByShopIdAsync(shopId);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(productDtos);
        }

    }
}
