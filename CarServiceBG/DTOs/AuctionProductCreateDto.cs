namespace CarServiceBG.DTOs
{
    public class AuctionProductCreateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal StartPrice { get; set; }
        public IFormFile? Photo { get; set; }

    }
}
