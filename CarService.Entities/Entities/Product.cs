namespace CarService.Entities.Entities
{
    public class Product
    {
        public Guid? Id { get; set; }
        public string? UserId { get; set; }
        public Guid? ShopId { get; set; }
        public string? ShopCategory { get; set; }
        public string? Name { get; set; }
        public decimal? StockQuantity { get; set; }
        public decimal? Point { get; set; }
        public decimal? Price { get; set; }
        public string? ProductImage { get; set; }
    }
}
