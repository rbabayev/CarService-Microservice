namespace CarServiceBG.DTOs
{
    public class AuctionProductResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public string? PhotoUrl { get; set; }
        public string SellerName { get; set; }
    }
}
