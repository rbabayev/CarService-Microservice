namespace CarService.Entities.Entities
{
    public class Shop
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string ShopName { get; set; }
        public string ShopCategory { get; set; }
        public string? ShopImageUrl { get; set; }
        public decimal Point { get; set; }
    }
}
