namespace CarServiceBG.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DateTime { get; set; }
        public UserDto? User { get; set; }
        public List<CommentDto>? Comments { get; set; }
    }
}
