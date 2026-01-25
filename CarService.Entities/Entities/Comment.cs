using CarService.Core.Abstract;

namespace CarService.Entities.Entities
{
    public class Comment : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string? UserName { get; set; }
        public decimal Point { get; set; }
        public DateTime DateTime { get; set; }
        public Guid UserId { get; set; }
        public Guid WorkerId { get; set; }
    }
}
