using CarService.Core.Abstract;

namespace CarService.Entities.Entities
{
    public class Post : IEntity
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DateTime { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
