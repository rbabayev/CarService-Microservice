using CarService.Core.Abstract;

namespace CarService.Entities.Entities
{
    public class Message : IEntity
    {
        public int Id { get; set; }
        public string? MessageTxt { get; set; }
        public DateTime DateTime { get; set; }
        public int ChatId { get; set; }
        public virtual Chat? Chat { get; set; }
        public Guid? UserId { get; set; }
        public Guid? RecipientUserId { get; set; }
        public virtual User? User { get; set; }
        public virtual User? RecipientUser { get; set; }
    }
}
