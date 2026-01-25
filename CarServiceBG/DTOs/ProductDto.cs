namespace CarServiceBG.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Name { get; set; }
        public decimal? StockQuantity { get; set; }
        public decimal? Price { get; set; }
        public string? ProductImage { get; set; }
        public IFormFile? Photo { get; set; }

    }
}
