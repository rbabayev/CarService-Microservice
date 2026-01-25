namespace CarServiceBG.DTOs
{
    public class RegisterDto
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public string Role { get; set; }
        public string? ShopCategory { get; set; }
        public decimal? ExperienceYears { get; set; }
        public string? WorkerCategory { get; set; }
        public string? WorkerName { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ShopName { get; set; }
    }
}
