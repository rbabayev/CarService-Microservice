using System.ComponentModel.DataAnnotations.Schema;

namespace CarService.Entities.Entities
{
    public class AuctionTransaction
    {
        public int Id { get; set; }

        public Guid AuctionProductId { get; set; }

        [ForeignKey(nameof(AuctionProductId))]
        public AuctionProduct AuctionProduct { get; set; }

        public Guid WinnerUserId { get; set; }

        [ForeignKey(nameof(WinnerUserId))]
        public User WinnerUser { get; set; }

        public decimal FinalPrice { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
