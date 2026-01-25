namespace CarService.Entities.Entities
{
    public class Worker
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? WorkerCategory { get; set; }
        public string? ProfileImageUrl { get; set; }
        public decimal? WorkerPoint { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Comment>? Comments { get; set; }

    }
}
