using CarServiceBG.DTOs;

namespace CarServiceBG.Services.Abstract
{
    public interface IAuctionService
    {
        Task<bool> CreateProductAsync(AuctionProductCreateDto dto, string sellerId);
        Task<bool> PlaceBidAsync(AuctionBidDto dto);
        Task<List<AuctionProductResponseDto>> GetAllActiveProductsAsync();
        Task<List<AuctionProductResponseDto>> GetAllVisibleProductsAsync();
        Task<AuctionProductResponseDto?> GetByIdAsync(Guid productId);
        Task<List<AuctionProductResponseDto>> GetPendingAndActiveAuctionsAsync();
        Task ActivateAuctionAsync(Guid auctionId);
        Task EndAuctionAsync(Guid auctionId);
        Task HandleAuctionStartAsync(Guid productId);
        Task HandleAuctionEndAsync(Guid productId);
        Task<bool> DeleteAuctionAsync(Guid auctionId);
        // IAuctionService.cs
        Task CheckAndActivateAuctionsAsync();
        Task CheckAndEndAuctionsAsync();

    }

}
