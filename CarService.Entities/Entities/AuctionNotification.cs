using System.ComponentModel.DataAnnotations;

namespace CarService.Entities.Entities
{
    public class AuctionNotification
    {
        [Key]
        public int Id { get; set; }

        public Guid ReceiverId { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
