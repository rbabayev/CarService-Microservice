namespace CarServiceBG.DTOs
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? ShopImageUrl { get; set; }
        public string? Role { get; set; }
    }
}
