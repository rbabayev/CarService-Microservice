using CarServiceBG.DTOs;
using CarServiceBG.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpPost("CreateProduct")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] AuctionProductCreateDto dto)
        {
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sellerId)) return Unauthorized("Seller ID missing.");

            var result = await _auctionService.CreateProductAsync(dto, sellerId);
            return result ? Ok("Product created") : BadRequest("Creation failed");
        }

        [HttpPost("PlaceBid")]
        [Authorize(Roles = "User,Seller,Admin")]
        public async Task<IActionResult> PlaceBid([FromBody] AuctionBidDto dto)
        {
            var result = await _auctionService.PlaceBidAsync(dto);
            return result ? Ok(true) : BadRequest(false);
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetActive()
        {
            var products = await _auctionService.GetAllActiveProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _auctionService.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpGet("visible")]
        public async Task<IActionResult> GetVisible()
        {
            var products = await _auctionService.GetAllVisibleProductsAsync();
            return Ok(products);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("end/{id}")]
        public async Task<IActionResult> EndAuction(Guid id)
        {
            await _auctionService.EndAuctionAsync(id);
            return Ok("Auction ended by admin.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(Guid id)
        {
            var result = await _auctionService.DeleteAuctionAsync(id);
            return result ? Ok("Auction deleted.") : NotFound("Auction not found.");
        }


    }
}
