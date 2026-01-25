namespace CarServiceBG.DTOs
{
    public class ShopDto
    {
        public Guid Id { get; set; }
        public string ShopName { get; set; }
        public decimal Point { get; set; }
        public string? ShopImageUrl { get; set; }
    }
}
