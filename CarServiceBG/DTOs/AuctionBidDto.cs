namespace CarServiceBG.DTOs
{
    public class AuctionBidDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Amount { get; set; }
    }
}
