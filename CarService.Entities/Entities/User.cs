using CarService.Core.Abstract;
using Microsoft.AspNetCore.Identity;

namespace CarService.Entities.Entities
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public decimal? ShopPoint { get; set; }
        public decimal? WorkerPoint { get; set; }
        public string? ShopName { get; set; }
        public string? WorkerName { get; set; }
        public string? ShopCategory { get; set; }
        public string? WorkerCategory { get; set; }
        public string? ChatRoom { get; set; }
        public decimal? ExperienceYears { get; set; }
        public string Role { get; set; }
        public string? Status { get; set; }
        public string? ShopImageUrl { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }
        public string? ProfileImageUrl { get; set; }
        public Guid? ShopId { get; set; }
        public Shop? Shop { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Product>? Products { get; set; }


    }
}
