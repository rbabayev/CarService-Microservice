using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarService.Entities.Entities
{
    public class AuctionProduct
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }

        [Required]
        public decimal StartPrice { get; set; }

        [Required]
        public decimal CurrentPrice { get; set; }

        public string? PhotoUrl { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public string Status { get; set; } // "Active", "Ended", etc.

        [Required]
        public Guid SellerId { get; set; }

        [ForeignKey("SellerId")]
        public User Seller { get; set; }
        public ICollection<AuctionBid> Bids { get; set; } = new List<AuctionBid>();


    }
}
