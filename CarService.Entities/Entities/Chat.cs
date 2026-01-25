using CarService.Core.Abstract;

namespace CarService.Entities.Entities
{
    public class Chat : IEntity
    {
        public int Id { get; set; }
        public Guid? ReceiverId { get; set; }
        public virtual User? Receiver { get; set; }
        public Guid? SenderId { get; set; }
        public virtual List<Message>? Messages { get; set; }
        public Chat()
        {
            Messages = new List<Message>();
        }
    }
}
