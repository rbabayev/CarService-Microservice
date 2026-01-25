namespace CarServiceBG.DTOs
{
    public class UserPhotoUpdateDto
    {
        public Guid UserId { get; set; }
        public IFormFile Photo { get; set; }
    }
}
